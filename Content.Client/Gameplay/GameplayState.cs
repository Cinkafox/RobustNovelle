using Content.Client.Gameplay.UI;
using Content.Client.GameTicking;
using Content.Client.UserInterface.Systems;
using Robust.Client;
using Robust.Client.GameObjects;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.Input;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Client.Gameplay;

public sealed class GameplayState : State
{
    private readonly GameplayStateLoadController _loadController;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IEntitySystemManager _entitySystemManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IInputManager _inputManager = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly IBaseClient _client = default!;

    public GameplayState()
    {
        IoCManager.InjectDependencies(this);

        _loadController = _uiManager.GetUIController<GameplayStateLoadController>();
    }

    protected override void Startup()
    {
        _client.StartSinglePlayer();
        _uiManager.LoadScreen<DefaultGameScreen>();
        _loadController.LoadScreen();
        _entitySystemManager.GetEntitySystem<GameTicker>().SpawnPlayer(_playerManager.LocalSession!);
        _inputManager.KeyBindStateChanged += InputManagerOnKeyBindStateChanged;
    }

    protected override void Shutdown()
    {
        _uiManager.UnloadScreen();
        _loadController.UnloadScreen();
        _inputManager.KeyBindStateChanged -= InputManagerOnKeyBindStateChanged;
        _client.StopSinglePlayer();
    }
    
    private void InputManagerOnKeyBindStateChanged(ViewportBoundKeyEventArgs args)
    {
        // If there is no InputSystem, then there is nothing to forward to, and nothing to do here.
        if (!_entitySystemManager.TryGetEntitySystem(out InputSystem? inputSys))
            return;

        var kArgs = args.KeyEventArgs;
        var func = kArgs.Function;
        var funcId = _inputManager.NetworkBindMap.KeyFunctionID(func);

        EntityCoordinates coordinates = default;
        if (args.Viewport is IViewportControl vp)
        {
            var mousePosWorld = vp.PixelToMap(kArgs.PointerLocation.Position);
            coordinates = _mapManager.TryFindGridAt(mousePosWorld, out _, out var grid)
                ? grid.MapToGrid(mousePosWorld)
                : EntityCoordinates.FromMap(_mapManager, mousePosWorld);
        }

        var message = new ClientFullInputCmdMessage(_timing.CurTick, _timing.TickFraction, funcId)
        {
            State = kArgs.State,
            Coordinates = coordinates,
            ScreenCoordinates = kArgs.PointerLocation,
            Uid = default
        }; 

        // client side command handlers will always be sent the local player session.
        var session = _playerManager.LocalSession;
        if (inputSys.HandleInputCommand(session, func, message)) kArgs.Handle();
    }
}
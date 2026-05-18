using Content.Client.Gameplay.UI;
using Content.Client.GameTicking;
using Content.Client.UserInterface.Systems;
using Robust.Client.GameObjects;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Input;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Client.Gameplay;

public sealed partial class GameplayState : State
{
    [Dependency] private IEntitySystemManager _entitySystemManager = default!;
    [Dependency] private IInputManager _inputManager = default!;
    [Dependency] private IMapManager _mapManager = default!;
    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private IUserInterfaceManager _uiManager = default!;
    
    private TransformSystem _transformSystem;
    private MapSystem _mapSystem;
    
    private readonly GameplayStateLoadController _loadController;
    
    public GameplayState()
    {
        IoCManager.InjectDependencies(this);

        _loadController = _uiManager.GetUIController<GameplayStateLoadController>();
        _transformSystem = _entitySystemManager.GetEntitySystem<TransformSystem>();
        _mapSystem = _entitySystemManager.GetEntitySystem<MapSystem>();
    }

    protected override void Startup()
    {
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
            coordinates = _mapManager.TryFindGridAt(mousePosWorld, out var uid, out var grid)
                ? _mapSystem.MapToGrid(uid, mousePosWorld)
                : _transformSystem.ToCoordinates(mousePosWorld);
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
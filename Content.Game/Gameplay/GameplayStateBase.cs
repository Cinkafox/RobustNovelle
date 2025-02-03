using Content.Game.Gameplay.UI;
using Content.Game.Location.Systems;
using Content.Game.Scene.Manager;
using Content.Game.UserInterface.Controls;
using Content.Game.UserInterface.Systems;
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

namespace Content.Game.Gameplay;

[Virtual]
public class GameplayStateBase : State
{
    private readonly GameplayStateLoadController _loadController;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    [Dependency] private readonly ISceneManager _sceneManager = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IEntitySystemManager _entitySystemManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IInputManager _inputManager = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;

    public GameplayStateBase()
    {
        IoCManager.InjectDependencies(this);

        _loadController = _uiManager.GetUIController<GameplayStateLoadController>();
    }

    public MainViewport Viewport => _uiManager.ActiveScreen!.GetWidget<MainViewport>()!;

    protected override void Startup()
    {
        _uiManager.LoadScreen<DefaultGameScreen>();
        _loadController.LoadScreen();
        _sceneManager.Initialize();
        _sceneManager.LoadScene(_cfg.GetCVar(CCVars.CCVars.LastScenePrototype));
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
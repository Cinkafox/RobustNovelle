using Cinka.Game.Camera;
using Cinka.Game.Gameplay.UI;
using Cinka.Game.UserInterface.Controls;
using Cinka.Game.UserInterface.Systems;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Timing;
using InputSystem = Robust.Client.GameObjects.InputSystem;

namespace Cinka.Game.Gameplay;

[Virtual]
public class GameplayStateBase : State
{
    private readonly GameplayStateLoadController _loadController;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    [Dependency] private readonly IInputManager _inputManager = default!;
    [Dependency] private readonly IEntitySystemManager _entitySystemManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
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
        _inputManager.KeyBindStateChanged += OnKeyBindStateChanged;
        _loadController.LoadScreen();
    }

    protected override void Shutdown()
    {
        _uiManager.UnloadScreen();
        _inputManager.KeyBindStateChanged -= OnKeyBindStateChanged;
        _loadController.UnloadScreen();
    }
    
    
    protected virtual void OnKeyBindStateChanged(ViewportBoundKeyEventArgs args)
    {
        // If there is no InputSystem, then there is nothing to forward to, and nothing to do here.
        if(!_entitySystemManager.TryGetEntitySystem(out InputSystem? inputSys))
            return;

        var kArgs = args.KeyEventArgs;
        var func = kArgs.Function;
        var funcId = _inputManager.NetworkBindMap.KeyFunctionID(func);

        EntityCoordinates coordinates = default;
        //EntityUid? entityToClick = null;
        if (args.Viewport is IViewportControl vp)
        {
            var mousePosWorld = vp.PixelToMap(kArgs.PointerLocation.Position);
            //entityToClick = GetClickedEntity(mousePosWorld);

            coordinates = _mapManager.TryFindGridAt(mousePosWorld, out _, out var grid) ?
                grid.MapToGrid(mousePosWorld) :
                EntityCoordinates.FromMap(_mapManager, mousePosWorld);
        }

        var message = new ClientFullInputCmdMessage(_timing.CurTick, _timing.TickFraction, funcId)
        {
            State = kArgs.State,
            Coordinates = coordinates,
            ScreenCoordinates = kArgs.PointerLocation,
            //Uid = entityToClick ?? default,
        }; 

        // client side command handlers will always be sent the local player session.
        var session = _playerManager.LocalSession;
        if (inputSys.HandleInputCommand(session, func, message))
        {
            kArgs.Handle();
        }
    }
}
using Cinka.Game.Camera.Manager;
using Cinka.Game.Gameplay.UI;
using Cinka.Game.Location.Managers;
using Cinka.Game.Scene.Manager;
using Cinka.Game.StyleSheet;
using Cinka.Game.UserInterface.Controls;
using Cinka.Game.UserInterface.Systems;
using Robust.Client;
using Robust.Client.Input;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Cinka.Game.Gameplay;

[Virtual]
public class GameplayStateBase : State
{
    private readonly GameplayStateLoadController _loadController;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    [Dependency] private readonly ICameraManager _cameraManager = default!;
    [Dependency] private readonly ILocationManager _locationManager = default!;
    [Dependency] private readonly ISceneManager _sceneManager = default!;
    [Dependency] private readonly IStylesheetManager _stylesheetManager = default!;

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
        
        _locationManager.Initialize();
        _cameraManager.Initialize();
        _sceneManager.Initialize();
    }

    protected override void Shutdown()
    {
        _uiManager.UnloadScreen();
        _loadController.UnloadScreen();
    }
}
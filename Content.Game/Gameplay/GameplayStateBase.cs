using Content.Game.Camera.Manager;
using Content.Game.Gameplay.UI;
using Content.Game.Location.Managers;
using Content.Game.Scene.Manager;
using Content.Game.StyleSheet;
using Content.Game.UserInterface.Controls;
using Content.Game.UserInterface.Systems;
using Robust.Client;
using Robust.Client.Input;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Game.Gameplay;

[Virtual]
public class GameplayStateBase : State
{
    private readonly GameplayStateLoadController _loadController;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    [Dependency] private readonly ICameraManager _cameraManager = default!;
    [Dependency] private readonly ILocationManager _locationManager = default!;
    [Dependency] private readonly ISceneManager _sceneManager = default!;

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
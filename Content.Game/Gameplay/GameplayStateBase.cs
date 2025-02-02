using Content.Game.Gameplay.UI;
using Content.Game.Location.Systems;
using Content.Game.Scene.Manager;
using Content.Game.UserInterface.Controls;
using Content.Game.UserInterface.Systems;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;

namespace Content.Game.Gameplay;

[Virtual]
public class GameplayStateBase : State
{
    private readonly GameplayStateLoadController _loadController;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    [Dependency] private readonly ISceneManager _sceneManager = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;

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
    }

    protected override void Shutdown()
    {
        _uiManager.UnloadScreen();
        _loadController.UnloadScreen();
    }
}
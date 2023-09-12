using Cinka.Game.Gameplay.UI;
using Cinka.Game.UserInterface.Controls;
using Cinka.Game.UserInterface.Systems;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Analyzers;
using Robust.Shared.IoC;

namespace Cinka.Game.Gameplay;

[Virtual]
public class GameplayStateBase : State
{
    private readonly GameplayStateLoadController _loadController;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;

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
    }

    protected override void Shutdown()
    {
        _uiManager.UnloadScreen();
        _loadController.UnloadScreen();
    }
}
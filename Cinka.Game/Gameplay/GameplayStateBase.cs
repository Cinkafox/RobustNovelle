using Cinka.Game.UserInterface.Controls;
using Cinka.Game.UserInterface.Systems;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using DefaultGameScreen = Cinka.Game.Gameplay.UI.DefaultGameScreen;

namespace Cinka.Game.Gameplay;

[Virtual]
public class GameplayStateBase : State
{
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    
    public MainViewport Viewport => _uiManager.ActiveScreen!.GetWidget<MainViewport>()!;
    
    private readonly GameplayStateLoadController _loadController;

    public GameplayStateBase()
    {
        IoCManager.InjectDependencies(this);
        
        _loadController = _uiManager.GetUIController<GameplayStateLoadController>();
    }
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
using Cinka.Game.Gameplay.UI;
using Cinka.Game.UserInterface.Controls;
using Cinka.Game.UserInterface.Systems;
using Cinka.Game.Viewport;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Analyzers;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Cinka.Game.Gameplay;

[Virtual]
public class GameplayState : GameplayStateBase, IMainViewportState
{
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;
    [Dependency] private readonly IEyeManager _eyeManager = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    private readonly GameplayStateLoadController _loadController;
    [Dependency] private readonly IOverlayManager _overlayManager = default!;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;

    private readonly FpsCounter _fpsCounter = default!;

    public GameplayState()
    {
        IoCManager.InjectDependencies(this);

        _loadController = _uiManager.GetUIController<GameplayStateLoadController>();
    }

    public MainViewport Viewport => _uiManager.ActiveScreen!.GetWidget<MainViewport>()!;

    protected override void Startup()
    {
        base.Startup();

        LoadMainScreen();
    }

    protected override void Shutdown()
    {
        base.Shutdown();
        _eyeManager.MainViewport = UserInterfaceManager.MainViewport;
        _fpsCounter.Dispose();
        _uiManager.ClearWindows();
        UnloadMainScreen();
    }

    private void ReloadMainScreenValueChange(string _)
    {
        ReloadMainScreen();
    }

    public void ReloadMainScreen()
    {
        if (_uiManager.ActiveScreen?.GetWidget<MainViewport>() == null) return;

        UnloadMainScreen();
        LoadMainScreen();
    }

    private void UnloadMainScreen()
    {
        _loadController.UnloadScreen();
        _uiManager.UnloadScreen();
    }

    private void LoadMainScreen()
    {
        _uiManager.LoadScreen<DefaultGameScreen>();
        _loadController.LoadScreen();
    }

    protected override void OnKeyBindStateChanged(ViewportBoundKeyEventArgs args)
    {
        if (args.Viewport == null)
            base.OnKeyBindStateChanged(new ViewportBoundKeyEventArgs(args.KeyEventArgs, Viewport.Viewport));
        else
            base.OnKeyBindStateChanged(args);
    }
}
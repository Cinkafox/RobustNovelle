using Content.Client.Menu.UI;
using Robust.Client.State;
using Robust.Client.UserInterface;

namespace Content.Client.Menu;

public sealed partial class MenuState : State
{
    [Dependency] private IUserInterfaceManager _uiManager = default!;

    protected override void Startup()
    {
        _uiManager.LoadScreen<MenuScreen>();
    }

    protected override void Shutdown()
    {
        _uiManager.UnloadScreen();
    }
}
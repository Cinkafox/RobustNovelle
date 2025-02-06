using Content.Client.Menu.UI;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;

namespace Content.Client.Menu;

public sealed class MenuState : State
{
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    
    protected override void Startup()
    {
        _uiManager.LoadScreen<MenuScreen>();
    }

    protected override void Shutdown()
    {
        _uiManager.UnloadScreen();
    }
}
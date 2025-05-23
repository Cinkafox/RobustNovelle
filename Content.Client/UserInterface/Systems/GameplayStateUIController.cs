using System;
using Robust.Client.UserInterface.Controllers;

namespace Content.Client.UserInterface.Systems;

public sealed class GameplayStateLoadController : UIController
{
    public Action? OnScreenLoad;
    public Action? OnScreenUnload;

    public void UnloadScreen()
    {
        OnScreenUnload?.Invoke();
    }

    public void LoadScreen()
    {
        OnScreenLoad?.Invoke();
    }
}
using System.Diagnostics;
using Cinka.Game.Background.Components;
using Cinka.Game.Gameplay;
using Cinka.Game.Gameplay.UI;
using Cinka.Game.UserInterface.Systems.Background;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;

namespace Cinka.Game.UserInterface.Systems;

public sealed class BackgroundUIController : UIController , IOnStateChanged<GameplayStateBase>
{
    public BackgroundControl? BackgroundControl { get; private set; }
   
    public void OnStateEntered(GameplayStateBase state)
    {
        Logger.Debug("EBLYA S FELINIDS");
        Debug.Assert(UIManager.ActiveScreen != null, "UIManager.ActiveScreen != null");
        var gameScreen = (DefaultGameScreen)UIManager.ActiveScreen;
        BackgroundControl = gameScreen.Background;
    }

    public void OnStateExited(GameplayStateBase state)
    {
        BackgroundControl = null;
    }

    public void Ensure(Entity<BackgroundComponent> entity)
    {
        Logger.Debug((BackgroundControl is null) + "<>M");
        BackgroundControl?.Ensure(entity.Comp._layer,entity.Owner.ToString());
    }

    public void Remove(Entity<BackgroundComponent?> entity)
    {
        BackgroundControl?.Remove(entity.Owner.ToString());
    }
}
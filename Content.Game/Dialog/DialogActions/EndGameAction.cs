using Content.Game.Dialog.Data;
using Content.Game.Menu;
using Robust.Client;
using Robust.Client.GameStates;
using Robust.Client.State;

namespace Content.Game.Dialog.DialogActions;

public sealed partial class EndGameAction : IDialogAction
{
    public void Act()
    {
        IoCManager.Resolve<IStateManager>().RequestStateChange<MenuState>();
    }
}
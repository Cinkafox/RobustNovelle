using Content.Client.Dialog.Data;
using Content.Client.Menu;
using Robust.Client;
using Robust.Client.GameStates;
using Robust.Client.State;

namespace Content.Client.Dialog.DialogActions;

public sealed partial class EndGameAction : IDialogAction
{
    public void Act()
    {
        IoCManager.Resolve<IStateManager>().RequestStateChange<MenuState>();
    }
}
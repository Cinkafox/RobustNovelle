using Content.Client.Dialog.Components;
using Content.Client.Dialog.Data;
using Robust.Client;

namespace Content.Client.Dialog.DialogActions;

public sealed partial class EndGameAction : IDialogAction
{
    public void Act(IDependencyCollection collection, Entity<DialogContainerComponent> actorUid)
    {
        collection.Resolve<IBaseClient>().StopSinglePlayer();
    }
}
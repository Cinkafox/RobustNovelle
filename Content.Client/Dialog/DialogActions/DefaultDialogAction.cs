using Content.Client.Dialog.Components;
using Content.Client.Dialog.Data;
using Content.Client.Dialog.Systems;
using JetBrains.Annotations;

namespace Content.Client.Dialog.DialogActions;

[UsedImplicitly]
public sealed partial class DefaultDialogAction : IDialogAction
{
    public void Act(IDependencyCollection collection, Entity<DialogContainerComponent> actorUid)
    {
        collection.Resolve<EntityManager>().SystemOrNull<DialogSystem>()?.ContinueDialog(actorUid);
    }
}
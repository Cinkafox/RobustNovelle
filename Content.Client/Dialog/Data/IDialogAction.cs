using Content.Client.Dialog.Components;

namespace Content.Client.Dialog.Data;

[ImplicitDataDefinitionForInheritors]
public partial interface IDialogAction
{
    void Act(IDependencyCollection collection, Entity<DialogContainerComponent> actorUid);
}
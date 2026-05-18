using Content.Client.Character.Systems;
using Content.Client.Dialog.Components;
using Content.Client.Dialog.Data;
using Robust.Shared.Prototypes;

namespace Content.Client.Dialog.DialogActions;

public sealed partial class ChangeCharacterState : IDialogAction
{
    [DataField] public EntProtoId Prototype = "default";

    [DataField] public string State = "default";

    public void Act(IDependencyCollection collection, Entity<DialogContainerComponent> actorUid)
    {
        collection.Resolve<EntityManager>().System<CharacterSystem>().SetCharacterState(actorUid, Prototype, State);
    }
}
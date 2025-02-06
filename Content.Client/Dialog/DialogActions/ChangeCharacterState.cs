using Content.Client.Character.Systems;
using Content.Client.Dialog.Data;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Dialog.DialogActions;

public sealed partial class ChangeCharacterState : IDialogAction
{
    [DataField] public EntProtoId Prototype = "default";

    [DataField] public string State = "default";

    public void Act()
    {
        IoCManager.Resolve<EntityManager>().System<CharacterSystem>().SetCharacterState(Prototype,State);
    }
}
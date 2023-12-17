using Cinka.Game.Character.Managers;
using Cinka.Game.Dialog.Data;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Dialog.DialogActions;

public sealed partial class ChangeCharacterState : IDialogAction
{
    [DataField] public EntProtoId Prototype = "default";

    [DataField] public string State = "default";

    public void Act()
    {
        IoCManager.Resolve<EntityManager>().System<CharacterSystem>().SetCharacterState(Prototype,State);
    }
}
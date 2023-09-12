using Cinka.Game.Character.Managers;
using Cinka.Game.Dialog.Data;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Dialog.DialogActions;

public sealed class ChangeCharacterState : IDialogAction
{
    [DataField("prototype")] public string Prototype = "default";

    [DataField("state")] public string State = "default";

    public void Act()
    {
        IoCManager.Resolve<ICharacterManager>().SetCharacterState(Prototype, State);
    }
}
using Content.Game.Dialog.DialogActions;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Game.Dialog.Data;

[DataDefinition]
public sealed partial class DialogButton
{
    [DataField] public string Name = "default";

    [DataField] public IDialogAction DialogAction = new DefaultDialogAction();
}
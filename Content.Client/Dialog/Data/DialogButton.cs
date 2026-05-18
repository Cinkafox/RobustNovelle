using Content.Client.Dialog.DialogActions;

namespace Content.Client.Dialog.Data;

[DataDefinition]
public sealed partial class DialogButton
{
    [DataField] public IDialogAction DialogAction = new DefaultDialogAction();
    [DataField] public string Name = "default";
}
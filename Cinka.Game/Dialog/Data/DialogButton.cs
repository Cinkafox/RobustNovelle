using Cinka.Game.Dialog.DialogActions;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Dialog.Data;

[DataDefinition]
public sealed class DialogButton
{
    [DataField("name")] public string Name = "default";

    [DataField("dialogAction")] public IDialogAction? OnAction = new DefaultDialogAction();
}
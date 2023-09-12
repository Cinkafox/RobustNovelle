using System.Collections.Generic;
using Cinka.Game.Dialog.DialogActions;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Cinka.Game.Dialog.Data;

[DataDefinition]
public sealed class Dialog
{
    [DataField("action")] public IDialogAction Action = new DefaultDialogAction();

    [DataField("actions")] public List<IDialogAction> Actions = new();

    [DataField("delay")] public float Delay = 70;

    [DataField("name")] public string? Name;

    [DataField("newDialog")] public bool NewDialog = true;

    [ViewVariables(VVAccess.ReadOnly)] public float PassedTime;

    [DataField("skipDialog")] public bool SkipDialog;

    [DataField("text")] public string Text = "";
}
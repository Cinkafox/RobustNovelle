using System;
using System.Collections.Generic;
using Cinka.Game.Dialog.DialogActions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Cinka.Game.Dialog.Data;

[DataDefinition]
public sealed class Dialog
{
    [DataField("name")] public string? Name;
    
    [DataField("text")]
    public string Text = "";
    
    [DataField("delay")]
    public float Delay = 70;
    
    [ViewVariables(VVAccess.ReadOnly)]
    public float PassedTime;

    [DataField("actions")]
    public List<IDialogAction> Actions = new();

    [DataField("action")] public IDialogAction Action = new DefaultDialogAction();
    
    [DataField("newDialog")] public bool NewDialog = true;
    [DataField("skipDialog")] public bool SkipDialog;
}

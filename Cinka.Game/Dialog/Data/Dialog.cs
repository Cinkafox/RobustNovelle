using System.Collections.Generic;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Cinka.Game.Dialog.Data;

[DataDefinition]
public sealed partial class Dialog
{
    [DataField] public List<IDialogAction> Actions = new();

    [DataField] public float Delay = 40;

    [DataField] public string? Character;

    [DataField] public string? Name;

    [DataField] public bool NewDialog = true;

    [DataField] public bool SkipDialog;

    [DataField] public bool DontLetSkip;
    
    [DataField] public bool IsDialog = true;

    [DataField] public string Text = " ";

    [DataField] public string Emote = "default";
    
    [ViewVariables(VVAccess.ReadOnly)] public float PassedTime;
    
    [DataField] public HashSet<DialogButton> Choices = new();
}
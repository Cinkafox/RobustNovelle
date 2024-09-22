using System.Collections.Generic;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Cinka.Game.Dialog.Data;

[DataDefinition]
public sealed partial class Dialog
{
    [DataField] public List<IDialogAction> Actions = new();
    [DataField] public float Delay = 30;
    [DataField] public string? Character;
    [DataField] public string? Name;
    [DataField] public string Text = " ";
    [DataField] public string Emote = "default";
    [DataField] public bool NewDialog = true;
    [DataField] public bool SkipDialog;
    [DataField] public bool DontLetSkip;
    [DataField] public bool IsDialog = true;
    [DataField] public bool SayLetters = true;
    [DataField] public int SkipSayCount = 1;
    
    [DataField] public HashSet<DialogButton> Choices = new();
    [ViewVariables(VVAccess.ReadOnly)] public float PassedTime;
    [ViewVariables(VVAccess.ReadOnly)] public int SkipCounter;
}
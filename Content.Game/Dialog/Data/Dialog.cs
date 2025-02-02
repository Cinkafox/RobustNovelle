using System.Collections.Generic;
using System.Numerics;
using Content.Game.Location.Data;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Game.Dialog.Data;

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
    [DataField] public ProtoId<LocationPrototype>? Location;
    [DataField] public string? Title = null;
    [DataField] public string? CameraOn;
    [DataField] public HashSet<CharacterDefinition>? Characters;
    
    [DataField] public HashSet<DialogButton> Choices = new();
    [ViewVariables(VVAccess.ReadOnly)] public float PassedTime;
    [ViewVariables(VVAccess.ReadOnly)] public int SkipCounter;
}

[DataDefinition]
public sealed partial class CharacterDefinition
{
    [DataField] public EntProtoId Entity;
    [DataField] public bool? Visible;
    [DataField] public Vector2? Goto;
}
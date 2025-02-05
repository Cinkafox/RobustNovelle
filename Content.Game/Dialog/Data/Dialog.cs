using System.Numerics;
using Content.Game.Location.Data;
using Robust.Shared.Prototypes;

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
    [DataField] public string? Title;
    [DataField] public string? CameraOn;
    [DataField] public string? Show;
    [DataField] public string? Hide;
    
    [DataField] public HashSet<DialogButton> Choices = new();
    [ViewVariables(VVAccess.ReadOnly)] public float PassedTime;
    [ViewVariables(VVAccess.ReadOnly)] public int SkipCounter;
}

[DataDefinition]
public sealed partial class EntityDefinition
{
    [DataField] public EntProtoId Entity;
    [DataField] public Vector2 Position = Vector2.Zero;
    [DataField] public Angle Rotation = Angle.Zero;
}
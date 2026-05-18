using System.Numerics;
using Content.Client.Location.Data;
using Content.Client.PasterString.Data;
using Robust.Shared.Prototypes;

namespace Content.Client.Dialog.Data;

[DataDefinition]
public sealed partial class Dialog
{
    [DataField] public List<IDialogAction> Actions = new();
    [DataField] public string? CameraOn;
    [DataField] public SmartString? Character;

    [DataField] public HashSet<DialogButton> Choices = new();
    [DataField] public float Delay = 30;
    [DataField] public bool DontLetSkip;
    [DataField] public string? Emote;
    [DataField] public string? Hide;
    [DataField] public bool IsDialog = true;
    [DataField] public ProtoId<LocationPrototype>? Location;
    [DataField] public SmartString? Name;
    [DataField] public bool NewDialog = true;
    [ViewVariables(VVAccess.ReadOnly)] public float PassedTime;
    [DataField] public bool SayLetters = true;
    [DataField] public string? Show;
    [ViewVariables(VVAccess.ReadOnly)] public int SkipCounter;
    [DataField] public bool SkipDialog;
    [DataField] public int SkipSayCount = 1;
    [DataField] public SmartString Text = " ";
    [DataField] public SmartString? Title;
}

[DataDefinition]
public sealed partial class EntityDefinition
{
    [DataField] public EntProtoId Entity;
    [DataField] public Vector2 Position = Vector2.Zero;
    [DataField] public Angle Rotation = Angle.Zero;
}
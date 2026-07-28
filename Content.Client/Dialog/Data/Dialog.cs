using System.Numerics;
using Content.Client.Location.Data;
using Content.Client.PasterString.Data;
using Content.Client.Scene.Data;
using Robust.Shared.Prototypes;

namespace Content.Client.Dialog.Data;

[DataDefinition]
public sealed partial class Dialog
{
    [DataField] public IDialogAction? Action;
    [DataField] public ProtoId<LocationPrototype>? Location;
    [DataField] public ProtoId<ScenePrototype>? Scene;
    [DataField] public Dictionary<string, List<Dialog>> Choices = [];
    
    [DataField] public bool DontLetSkip;
    [DataField] public bool IsDialog = true;
    [DataField] public bool NewDialog = true;
    [DataField] public bool SayLetters = true;
    [DataField] public bool SkipDialog;
    [DataField] public bool StopDialog;
    
    [DataField] public int SkipSayCount = 1;
    [DataField] public float Delay = 30;
    
    [DataField] public string? CameraOn;
    [DataField] public string? Show;
    [DataField] public string? Emote;
    [DataField] public string? Hide;
    
    [DataField] public SmartString Text = " ";
    [DataField] public SmartString? Title;
    [DataField] public SmartString? Name;
    [DataField] public SmartString? Character;

    [DataField] public string? If;
    [DataField] public Dialog[]? Then;
    [DataField] public Dialog[]? Else;
    
    [DataField] public VariableDefinition? Variable;
}

[DataDefinition]
public sealed partial class VariableDefinition
{
    [DataField] public string Name;
    [DataField] public float Value;
}

[DataDefinition]
public sealed partial class EntityDefinition
{
    [DataField] public EntProtoId Entity;
    [DataField] public Vector2 Position = Vector2.Zero;
    [DataField] public Angle Rotation = Angle.Zero;
}
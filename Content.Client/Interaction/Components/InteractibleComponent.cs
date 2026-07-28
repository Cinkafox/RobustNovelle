using Content.Client.PasterString.Data;
using Content.Client.Scene.Data;
using Robust.Shared.Prototypes;

namespace Content.Client.Interaction.Components;

[RegisterComponent]
public sealed partial class InteractibleComponent : Component
{
    [DataField(required: true)] public ProtoId<ScenePrototype> Scene;
    [DataField] public bool InvokeImmediately;
    [DataField] public float MaxDistance = 1f;
    [DataField] public SmartString Name = "Взаимодействовать";
}
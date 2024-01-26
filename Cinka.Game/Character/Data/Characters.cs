using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Character.Data;

[DataDefinition]
public sealed partial class Character
{
    [DataField] public EntProtoId Entity;
    [DataField] public bool IsPlayer;
    [DataField] public Vector2? Position;
    [DataField("components")]
    [AlwaysPushInheritance]
    public ComponentRegistry Components { get; private set; } = new();
} 
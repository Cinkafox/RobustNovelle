using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Game.Location.Data;

[Prototype("location")]
public sealed class LocationPrototype : IPrototype
{
    [IdDataField] public string ID { get; private set; } = default!;
    
    [DataField] public EntProtoId Background = string.Empty;
}
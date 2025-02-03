using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Game.Location.Data;

[Prototype("location")]
public sealed class LocationPrototype : IPrototype
{
    [IdDataField] public string ID { get; private set; } = default!;
    
    [DataField] public EntProtoId? Background;
    [DataField] public LocationDefinition? Location;
}

[DataDefinition]
public sealed partial class LocationDefinition
{
    [DataField] public ResPath Path;
    [DataField] public ResPath? Map;
}
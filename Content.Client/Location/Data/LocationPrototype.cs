using Content.Client.Dialog.Data;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Location.Data;

[Prototype("location")]
public sealed class LocationPrototype : IPrototype
{
    [IdDataField] public string ID { get; private set; } = default!;
    
    [DataField] public ResPath? Background;
    [DataField] public LocationDefinition? Location;
    [DataField] public HashSet<EntityDefinition>? Entities;
}

[DataDefinition]
public sealed partial class LocationDefinition
{
    [DataField] public ResPath Path;
    [DataField] public ResPath? Map;
    [DataField] public ResPath? LightPath;
}
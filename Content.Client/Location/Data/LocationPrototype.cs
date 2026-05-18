using Content.Client.Dialog.Data;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Location.Data;

[Prototype]
public sealed partial class LocationPrototype : IPrototype
{
    [DataField] public HashSet<SoundSpecifier> AmbientSounds = [];

    [DataField] public ResPath? Background;
    [DataField] public HashSet<EntityDefinition>? Entities;
    [DataField] public LocationDefinition? Location;
    [IdDataField] public string ID { get; private set; } = default!;
}

[DataDefinition]
public sealed partial class LocationDefinition
{
    [DataField] public ResPath? LightPath;
    [DataField] public ResPath? Map;
    [DataField] public ResPath Path;
}
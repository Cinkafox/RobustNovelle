using Content.Client.Location.Data;
using Robust.Shared.Prototypes;

namespace Content.Client.Location.Components;

[RegisterComponent]
public sealed partial class LocationComponent : Component
{
    [ViewVariables] public HashSet<EntityUid> Ambients = [];
    [DataField] public LocationDefinition? CurrentLocation;

    [ViewVariables] public Dictionary<EntProtoId, EntityUid> EntityDefinitions = new();
}
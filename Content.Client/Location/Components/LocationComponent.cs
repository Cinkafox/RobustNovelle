using Content.Client.Location.Data;
using Robust.Shared.Prototypes;

namespace Content.Client.Location.Components;

[RegisterComponent]
public sealed partial class LocationComponent : Component
{
    [DataField] public LocationDefinition? CurrentLocation;
    
    [ViewVariables] public Dictionary<EntProtoId,EntityUid> EntityDefinitions = new();
    [ViewVariables] public HashSet<EntityUid> Ambients = [];
}
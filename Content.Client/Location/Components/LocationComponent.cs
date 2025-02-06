using Content.Client.Location.Data;

namespace Content.Client.Location.Components;

[RegisterComponent]
public sealed partial class LocationComponent : Component
{
    [DataField] public LocationDefinition CurrentLocation;
    
}
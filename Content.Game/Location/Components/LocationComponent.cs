using Content.Game.Location.Data;

namespace Content.Game.Location.Components;

[RegisterComponent]
public sealed partial class LocationComponent : Component
{
    [DataField] public LocationDefinition CurrentLocation;
    
}
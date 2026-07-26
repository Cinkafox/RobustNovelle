using Robust.Client.Graphics;
using Robust.Shared.Utility;

namespace Content.Client.Location.Components;

[RegisterComponent]
public sealed partial class LocationLightComponent: Component
{
    [DataField] public ResPath LightPath;
    public Texture? LightTexture;
}
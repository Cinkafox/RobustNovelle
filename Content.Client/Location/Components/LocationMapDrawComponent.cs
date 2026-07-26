using Robust.Client.Graphics;
using Robust.Shared.Utility;

namespace Content.Client.Location.Components;

[RegisterComponent]
public sealed partial class LocationMapDrawComponent: Component
{
    [DataField] public ResPath Path;
    public Texture? Texture;
}
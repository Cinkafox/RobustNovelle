using Robust.Client.Graphics;
using Robust.Shared.Animations;

namespace Content.Client.Background.Components;

[RegisterComponent]
public sealed partial class BackgroundComponent : Component
{
    public Texture Layer;
    
    //Some visibility shit. 0 - Not visible and 255 is visible
    [Animatable]
    [ViewVariables]
    public int Visibility { get; set; } = 255;
}
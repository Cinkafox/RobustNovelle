using Robust.Client.Graphics;

namespace Content.Client.Location.Systems;

public sealed partial class LocationRenderSystem : EntitySystem
{
    [Dependency] private IOverlayManager _overlayManager = default!;

    public override void Initialize()
    {
        _overlayManager.AddOverlay(new LocationOverlay());
        _overlayManager.AddOverlay(new LocationLightOverlay());
    }
}
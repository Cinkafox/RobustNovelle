using Robust.Client.Graphics;

namespace Content.Client.Location.Systems;

public sealed class LocationRenderSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlayManager = default!;

    public override void Initialize()
    {
        _overlayManager.AddOverlay(new LocationOverlay());
    }
}
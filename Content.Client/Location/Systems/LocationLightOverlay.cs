using Content.Client.Location.Components;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;

namespace Content.Client.Location.Systems;

public sealed class LocationLightOverlay : Overlay
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IResourceCache _resCache = default!;

    public LocationLightOverlay()
    {
        IoCManager.InjectDependencies(this);
    }
    
    public override OverlaySpace Space => OverlaySpace.WorldSpaceBelowFOV;
    protected override void Draw(in OverlayDrawArgs args)
    {
        var query = _entityManager.EntityQueryEnumerator<TransformComponent, LocationComponent>();
        while (query.MoveNext(out var transform, out var locationComponent))
        {
            if (locationComponent.CurrentLocation is { LightPath: not null })
            {
                if(args.Viewport.Eye?.Position.MapId != transform.MapID) continue;
                
                var tex = _resCache.GetResource<TextureResource>(locationComponent.CurrentLocation.LightPath.Value).Texture;
                args.WorldHandle.DrawTexture(tex, transform.WorldPosition);
            }
        }
    }
}
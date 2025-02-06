using Content.Client.Location.Components;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;

namespace Content.Client.Location.Systems;

public sealed class LocationOverlay : Overlay
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IResourceCache _resCache = default!;

    public LocationOverlay()
    {
        IoCManager.InjectDependencies(this);
    }
    
    public override OverlaySpace Space => OverlaySpace.WorldSpaceBelowWorld;
    protected override void Draw(in OverlayDrawArgs args)
    {
        var query = _entityManager.EntityQueryEnumerator<TransformComponent, LocationComponent>();
        while (query.MoveNext(out var transform, out var locationComponent))
        {
            var tex = _resCache.GetResource<TextureResource>(locationComponent.CurrentLocation.Path).Texture;
            args.WorldHandle.DrawTexture(tex, transform.WorldPosition);
        }
    }
}
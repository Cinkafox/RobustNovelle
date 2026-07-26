using System.Numerics;
using Content.Client.Location.Components;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;

namespace Content.Client.Location.Systems;

public sealed partial class LocationOverlay : Overlay
{
    [Dependency] private IEntityManager _entityManager = default!;
    [Dependency] private IResourceCache _resCache = default!;

    public LocationOverlay()
    {
        IoCManager.InjectDependencies(this);
    }

    public override OverlaySpace Space => OverlaySpace.WorldSpaceBelowWorld;

    protected override void Draw(in OverlayDrawArgs args)
    {
        if(!_entityManager.TryGetComponent<LocationMapDrawComponent>(args.MapUid, out var locationComponent)) 
            return;
        
        locationComponent.Texture ??= _resCache.GetResource<TextureResource>(locationComponent.Path).Texture;
        args.WorldHandle.DrawTexture(locationComponent.Texture, Vector2.Zero);
    }
}
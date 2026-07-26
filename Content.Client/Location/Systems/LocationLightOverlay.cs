using System.Numerics;
using Content.Client.Location.Components;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;

namespace Content.Client.Location.Systems;

public sealed partial class LocationLightOverlay : Overlay
{
    [Dependency] private IEntityManager _entityManager = default!;
    [Dependency] private IResourceCache _resCache = default!;

    public LocationLightOverlay()
    {
        IoCManager.InjectDependencies(this);
    }

    public override OverlaySpace Space => OverlaySpace.WorldSpaceBelowFOV;

    protected override void Draw(in OverlayDrawArgs args)
    {
        if(!_entityManager.TryGetComponent<LocationLightComponent>(args.MapUid, out var locationComponent)) 
            return;
        
        locationComponent.LightTexture ??= _resCache.GetResource<TextureResource>(locationComponent.LightPath).Texture;
        args.WorldHandle.DrawTexture(locationComponent.LightTexture, Vector2.Zero);
    }
}
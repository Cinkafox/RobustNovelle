using Robust.Client.Graphics;
using Robust.Shared.Enums;
using BackgroundComponent = Content.Client.Background.Components.BackgroundComponent;

namespace Content.Client.Background;

public sealed partial class BackgroundOverlay : Overlay
{
    [Dependency] private IEntityManager _entityManager = default!;

    public BackgroundOverlay()
    {
        ZIndex = BackgroundSystem.BackgroundZIndex;
        IoCManager.InjectDependencies(this);
    }

    public override OverlaySpace Space => OverlaySpace.WorldSpace;

    protected override void Draw(in OverlayDrawArgs args)
    {
        if(_entityManager.TryGetComponent<BackgroundComponent>(args.MapUid, out var background))
            args.WorldHandle.DrawTextureRect(background.Layer, args.WorldBounds.Box);
    }
}
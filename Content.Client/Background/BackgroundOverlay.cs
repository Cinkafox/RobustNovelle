using System.Linq;
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
        var parentUid = args.MapUid;
        //Почему не EntityQueryEnumerator? Потому что порядок рисовки сбивается.
        var list = _entityManager.EntityQuery<BackgroundComponent, TransformComponent>()
            .Where((b) => b.Item2.ParentUid == parentUid)
            .Select(b => b.Item1)
            .Reverse();

        foreach (var component in list) 
            DrawBackground(component.Layer, args, (byte)component.Visibility);
    }

    private void DrawBackground(Texture layer, OverlayDrawArgs args, byte alpha = 255)
    {
        var handle = args.WorldHandle;
        handle.DrawTextureRect(layer, args.WorldBounds.Box, new Color(255, 255, 255, alpha));
    }
}
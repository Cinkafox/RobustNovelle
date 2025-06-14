using System.Linq;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using BackgroundComponent = Content.Client.Background.Components.BackgroundComponent;

namespace Content.Client.Background;

public sealed class BackgroundOverlay : Overlay
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    public BackgroundOverlay()
    {
        ZIndex = BackgroundSystem.BackgroundZIndex;
        IoCManager.InjectDependencies(this);
    }

    public override OverlaySpace Space => OverlaySpace.WorldSpace;

    protected override void Draw(in OverlayDrawArgs args)
    {
        //Почему не EntityQueryEnumerator? Потому что порядок рисовки сбивается. И потому что иди нахуй да
        var list = _entityManager.EntityQuery<BackgroundComponent>().ToList();
        list.Reverse();
        
        foreach (var component in list)
        {
            DrawBackground(component.Layer,args,(byte)component.Visibility);
        }
    }

    public void DrawBackground(Texture layer, OverlayDrawArgs args,byte alpha = 255)
    {
        var handle = args.WorldHandle;
        handle.DrawTextureRect(layer, args.WorldBounds.Box,new Color(255,255,255,alpha));
    }
}
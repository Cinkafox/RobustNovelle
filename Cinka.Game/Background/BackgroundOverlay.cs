using Cinka.Game.Background.Manager;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;

namespace Cinka.Game.Background;

public sealed class BackgroundOverlay : Overlay
{
    [Dependency] private readonly IBackgroundManager _backgroundManager = default!;
    public override OverlaySpace Space => OverlaySpace.WorldSpaceBelowWorld;

    public BackgroundOverlay()
    {
        ZIndex = BackgroundSystem.BackgroundZIndex;
        IoCManager.InjectDependencies(this);
    }
    
    protected override void Draw(in OverlayDrawArgs args)
    {
        var handle = args.WorldHandle;
        
        var layers = _backgroundManager.GetCurrentBackground();
        
        foreach (var layer in layers)
        {
            handle.DrawTextureRect(layer,args.WorldBounds.Box);
        }
    }
}
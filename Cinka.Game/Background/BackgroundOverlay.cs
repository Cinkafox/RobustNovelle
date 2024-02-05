using Cinka.Game.Background.Manager;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Cinka.Game.Background;

public sealed class BackgroundOverlay : Overlay
{
    [Dependency] private readonly IBackgroundManager _backgroundManager = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    public BackgroundOverlay()
    {
        ZIndex = BackgroundSystem.BackgroundZIndex;
        IoCManager.InjectDependencies(this);
    }

    public override OverlaySpace Space => OverlaySpace.WorldSpaceBelowWorld;

    protected override void Draw(in OverlayDrawArgs args)
    {
        DrawBackground(_backgroundManager.GetCurrentBackground(),args);
        
        if (_backgroundManager.TryGetFadingBackground(out var layers))
        {
            var currTime = _gameTiming.CurTime - _backgroundManager.GetLastFadingBackgroundUpdateCurTime();
            var aa = (byte)(255-currTime.Milliseconds / 700f * 255);
            Logger.Debug((aa) + " <");
            DrawBackground(layers,args,aa);
            if (aa < 10)
            {
                _backgroundManager.ClearFadingBackground();
            }
        }
    }

    public void DrawBackground(Texture[] layers, OverlayDrawArgs args,byte alpha = 255)
    {
        var handle = args.WorldHandle;
        
        foreach (var layer in layers) handle.DrawTextureRect(layer, args.WorldBounds.Box,new Color(255,255,255,alpha));
    }
}
using Cinka.Game.Background.Manager;
using Cinka.Game.Viewport;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;

namespace Cinka.Game.Background;

public sealed class BackgroundOverlay : Overlay
{
    [Dependency] private readonly IInputManager _inputManager = default!;
    [Dependency] private readonly IBackgroundManager _backgroundManager = default!;
    
    public override OverlaySpace Space => OverlaySpace.WorldSpaceBelowWorld;

    public BackgroundOverlay()
    {
        ZIndex = BackgroundSystem.BackgroundZIndex;
        IoCManager.InjectDependencies(this);
    }
    
    protected override void Draw(in OverlayDrawArgs args)
    {
        var position = args.Viewport.Eye?.Position.Position ?? Vector2.Zero;
        var handle = args.WorldHandle;

        var layers = _backgroundManager.GetCurrentBackground();

        foreach (var layer in layers)
        {
            handle.DrawTextureRect(layer, args.WorldBounds.Box);
        }

    }
}
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Cinka.Game.Background;

public sealed class BackgroundOverlay : Overlay
{
    [Dependency] private readonly IInputManager _inputManager = default!;

    public BackgroundOverlay()
    {
        IoCManager.InjectDependencies(this);
    }
    
    protected override void Draw(in OverlayDrawArgs args)
    {
        var position = args.Viewport.Eye?.Position.Position ?? Vector2.Zero;
        var handle = args.ScreenHandle;
        

    }
}
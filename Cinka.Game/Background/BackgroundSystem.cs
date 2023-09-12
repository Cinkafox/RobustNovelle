using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Cinka.Game.Background;

public sealed class BackgroundSystem : EntitySystem
{
    public const int BackgroundZIndex = 0;
    [Dependency] private readonly IOverlayManager _overlay = default!;

    public override void Initialize()
    {
        _overlay.AddOverlay(new BackgroundOverlay());
    }
}
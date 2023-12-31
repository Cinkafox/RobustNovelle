using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Cinka.Game.CharacterRendering;

public sealed class CharacterRenderingSystem : EntitySystem
{
    public const int CharacterRenderingZIndex = 0;
    [Dependency] private readonly IOverlayManager _overlay = default!;

    public override void Initialize()
    {
        _overlay.AddOverlay(new CharacterRenderingOverlay());
    }
}
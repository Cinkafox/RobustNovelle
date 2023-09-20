using System.Numerics;
using Cinka.Game.Character;
using Cinka.Game.Character.Managers;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Cinka.Game.CharacterRendering;

public sealed class CharacterRenderingOverlay : Overlay
{
    private const float Shift = 2;
    [Dependency] private readonly ICharacterManager _characterManager = default!;

    private int _characterRendering;
    private float _elapsedTime;
    private int _frames;
    private float _lastDelta = 0.01f;

    public CharacterRenderingOverlay()
    {
        IoCManager.InjectDependencies(this);
    }

    public override OverlaySpace Space => OverlaySpace.WorldSpaceEntities;

    protected override void FrameUpdate(FrameEventArgs args)
    {
        _frames = _frames + 1;
        _elapsedTime += args.DeltaSeconds;
        _lastDelta = _elapsedTime / _frames;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        var handle = args.WorldHandle;
        _characterRendering = 0;

        foreach (var character in _characterManager.EnumerateCharacters())
            DrawCharacter(character, handle, args.WorldBounds, _characterManager.Count());
    }

    private void DrawCharacter(CharacterData character, DrawingHandleWorld handle, Box2Rotated bounds,
        int charactersCount)
    {
        var sprite = character.Sprite[character.State];
        var frames = sprite.GetFrames(0);
        var delay = sprite.GetDelay(_frames % frames.Length);
        var texture = frames[(int)(_frames * _lastDelta / delay) % frames.Length];

        var viewSize = bounds.TopRight * 2;

        var charCountChet = _characterRendering % 2;
        var shift = (1 - charCountChet * 2) * charactersCount * Shift - Shift * (1 - charCountChet);

        var hRatio = viewSize.Y / texture.Height;
        var wRatio = viewSize.X / texture.Width;

        var ratio = float.Min(hRatio, wRatio);

        var centerX = texture.Width * ratio / 2 + shift;

        handle.DrawTextureRect(texture,
            Box2.FromDimensions(-new Vector2(centerX, viewSize.Y / 2), texture.Size * ratio));

        _characterRendering++;
    }
}
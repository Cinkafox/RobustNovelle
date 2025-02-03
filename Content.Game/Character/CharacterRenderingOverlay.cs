using System.Linq;
using System.Numerics;
using Content.Game.Character.Components;
using Content.Game.Character.Systems;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Game.Character;

public sealed class CharacterRenderingOverlay : Overlay
{
    private const float Shift = 2;
    private readonly CharacterSystem _characterSystem;
    [Dependency] private readonly EntityManager _entityManager = default!;

    public static bool IsVisible = true;

    private int _characterRendering;
    private float _elapsedTime;
    private int _frames;
    private float _lastDelta = 0.01f;

    public CharacterRenderingOverlay()
    {
        IoCManager.InjectDependencies(this);
        _characterSystem = _entityManager.System<CharacterSystem>();
    }

    public override OverlaySpace Space => OverlaySpace.WorldSpace;

    protected override void FrameUpdate(FrameEventArgs args)
    {
        _frames += 1;
        _elapsedTime += args.DeltaSeconds;
        _lastDelta = _elapsedTime / _frames;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if(!IsVisible) return;
        
        var handle = args.WorldHandle;
        _characterRendering = 0;

        var characters = _characterSystem.EnumerateCharacters().ToList();

        foreach (var character in characters)
            DrawCharacter(character, handle, args.WorldBounds, characters.Count);
    }

    private void DrawCharacter(CharacterComponent character, DrawingHandleWorld handle, Box2Rotated bounds,
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
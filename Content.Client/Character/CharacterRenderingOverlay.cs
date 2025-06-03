using System.Linq;
using System.Numerics;
using Content.Client.Character.Components;
using Content.Client.Character.Systems;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Character;

public sealed class CharacterRenderingOverlay : Overlay
{
    private const float Shift = 2;
    private readonly CharacterSystem _characterSystem;
    [Dependency] private readonly EntityManager _entityManager = default!;

    public static bool IsVisible = true;
    
    private float _elapsedTime;
    private int _frames;
    private float _lastDelta = 0.01f;

    public CharacterRenderingOverlay()
    {
        IoCManager.InjectDependencies(this);
        _characterSystem = _entityManager.System<CharacterSystem>();
    }

    public override OverlaySpace Space => OverlaySpace.ScreenSpace;

    protected override void FrameUpdate(FrameEventArgs args)
    {
        _frames += 1;
        _elapsedTime += args.DeltaSeconds;
        _lastDelta = _elapsedTime / _frames;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if(!IsVisible) return;
        
        var handle = args.ScreenHandle;

        var characters = _characterSystem.EnumerateCharacters(args.MapUid).ToList();

        foreach (var character in characters)
            DrawCharacter(character, handle, args);
    }

    private void DrawCharacter(CharacterComponent character, DrawingHandleScreen handle, OverlayDrawArgs args)
    {
        var sprite = character.Sprite[character.State];
        var frames = sprite.GetFrames(0);
        var delay = sprite.GetDelay(_frames % frames.Length);
        var texture = frames[(int)(_frames * _lastDelta / delay) % frames.Length];

        var bounds = args.ViewportBounds;
        
        var width = texture.Width * (bounds.Height / (float)texture.Height);
        var xPos = character.XPosition;

        handle.DrawTextureRect(texture, UIBox2.FromDimensions(
            new Vector2((float)(args.ViewportControl!.Window!.Size.X * xPos - width / 2f),0), 
            new Vector2(width, bounds.Height)));
    }
}
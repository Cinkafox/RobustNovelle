using Cinka.Game.Character;
using Cinka.Game.Character.Managers;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Cinka.Game.CharacterRendering;

public sealed class CharacterRenderingOverlay : Overlay
{
    [Dependency] private readonly ICharacterManager _characterManager = default!;
    
    public override OverlaySpace Space => OverlaySpace.WorldSpaceEntities;
    private int _frames = 0;
    private float _elapsedTime = 0;
    private float _lastDelta = 0.01f;

    private int _characterRendering = 0;

    private static float _shift = 2;
    
    public CharacterRenderingOverlay()
    {
        IoCManager.InjectDependencies(this);
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        _frames = (_frames + 1) ;
        _elapsedTime += args.DeltaSeconds;
        _lastDelta = _elapsedTime / _frames;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        var handle = args.WorldHandle;
        _characterRendering = 0;
        
        foreach (var character in _characterManager.EnumerateCharacters())
        {
            DrawCharacter(character,handle,args.WorldBounds,_characterManager.Count());
        }
    }

    private void DrawCharacter(CharacterData character,DrawingHandleWorld handle,Box2Rotated bounds,int charactersCount)
    {
        var sprite = character.Sprite[character.State];
        var frames = sprite.GetFrames(0);
        var delay = sprite.GetDelay(_frames % frames.Length);
        var texture = frames[(int)(_frames*_lastDelta/delay) % frames.Length];
                
        var prop = (texture.Width / (float)texture.Height);
        var charCountChet = _characterRendering % 2;
        var shift = new Vector2((1 - charCountChet * 2) * charactersCount * _shift - _shift * (1-charCountChet), 0);
            
        handle.DrawTextureRect(texture,new Box2(
            bounds.BottomLeft * new Vector2(prop,1) + shift + new Vector2(1,0),
            bounds.TopRight* new Vector2(prop,1) + shift - new Vector2(1,0)
        ));
        
        _characterRendering++;
    }
}
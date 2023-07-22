using Cinka.Game.Character.Managers;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;

namespace Cinka.Game.CharacterRendering;

public sealed class CharacterRenderingOverlay : Overlay
{
    [Dependency] private readonly ICharacterManager _characterManager = default!;
    public override OverlaySpace Space => OverlaySpace.WorldSpaceEntities;

    public CharacterRenderingOverlay()
    {
        IoCManager.InjectDependencies(this);
    }
    
    protected override void Draw(in OverlayDrawArgs args)
    {
        var handle = args.WorldHandle;

        var lcount = 0f;
        
        foreach (var character in _characterManager.EnumerateCharacters())
        {
            var texture = character.Sprite[character.State].Frame0;
            var prop = (texture.Width / (float)texture.Height);

            var charcount = _characterManager.Count();
            var size = args.WorldBounds.BottomLeft.X ;
            var shift = new Vector2(size / charcount * lcount,0);
            handle.DrawTextureRect(texture,new Box2(
                args.WorldBounds.BottomLeft * new Vector2(prop,1) + shift,
                args.WorldBounds.TopRight* new Vector2(prop,1) + shift
                ));
            
            lcount++;
        }
    }
}
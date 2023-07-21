using Cinka.Game.Character.Managers;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
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

        foreach (var character in _characterManager.EnumerateCharacters())
        {
            var texture = character.Sprite[character.State].Frame0;
            
            handle.DrawTextureRect(texture,args.WorldBounds.Box);
        }
    }
}
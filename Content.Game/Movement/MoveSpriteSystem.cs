using Robust.Client.GameObjects;

namespace Content.Game.Movement;

public sealed class MoveSpriteSystem : EntitySystem
{
    [Dependency] private readonly SpriteSystem _spriteSystem = default!;
    
    public override void Initialize()
    {
        SubscribeLocalEvent<MoveSpriteComponent, OnEntityMoving>(OnMoving);
        SubscribeLocalEvent<MoveSpriteComponent, OnEntityStopMoving>(OnStopMoving);
        SubscribeLocalEvent<MoveSpriteComponent, ComponentInit>(OnInit);
    }

    private void OnInit(Entity<MoveSpriteComponent> ent, ref ComponentInit args)
    {
        if(!TryComp<SpriteComponent>(ent, out var spriteComponent))
        {
            RemComp(ent.Owner, ent.Comp);
            return;
        }

        spriteComponent.LayerSetVisible(MoveAnimationVisual.Walk, false);
    }

    private void OnStopMoving(Entity<MoveSpriteComponent> ent, ref OnEntityStopMoving args)
    {
        if(!TryComp<SpriteComponent>(ent, out var spriteComponent)) return; 
        
        spriteComponent.LayerSetVisible(MoveAnimationVisual.Walk, false);
        spriteComponent.LayerSetVisible(MoveAnimationVisual.Stay, true);
    }

    private void OnMoving(Entity<MoveSpriteComponent> ent, ref OnEntityMoving args)
    {
        if(!TryComp<SpriteComponent>(ent, out var spriteComponent)) return;
        
        spriteComponent.LayerSetVisible(MoveAnimationVisual.Walk, true);
        spriteComponent.LayerSetVisible(MoveAnimationVisual.Stay, false);
    }
}

public enum MoveAnimationVisual : byte
{
    Stay, Walk
}
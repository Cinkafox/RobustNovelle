using Content.Game.Interaction.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;

namespace Content.Game.Interaction.Systems;

public sealed class InteractionOverlay : Overlay
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    public InteractionOverlay()
    {
        IoCManager.InjectDependencies(this);
    }
    
    public override OverlaySpace Space => OverlaySpace.WorldSpace;
    protected override void Draw(in OverlayDrawArgs args)
    {
        var handle = args.WorldHandle;

        var query = _entityManager.EntityQueryEnumerator<InteractionComponent>();
        while (query.MoveNext(out var interactionComponent))
        {
            if(!interactionComponent.IsEnabled || interactionComponent.CurrentInteractible is null) continue;
            handle.DrawTexture(interactionComponent.IconTexture, 
                interactionComponent.CurrentInteractible.Value.Item2.LocalPosition);
        }
    }
}
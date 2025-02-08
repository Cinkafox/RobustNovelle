using System.Linq;
using Content.Client.Interaction.Components;
using Content.Client.Movement;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Client.Interaction.Systems;

public sealed class InteractionSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlayManager = default!;
    [Dependency] private readonly IResourceCache _resCache = default!;
    
    public override void Initialize()
    {
        _overlayManager.AddOverlay(new InteractionOverlay());
        
        SubscribeLocalEvent<InteractionComponent, ComponentInit>(OnInit);
        
        CommandBinds.Builder
            .Bind(EngineKeyFunctions.Use, new UseInretactionCommand(this))
            .Register<InteractionSystem>();
    }

    private void OnInit(Entity<InteractionComponent> ent, ref ComponentInit args)
    {
        ent.Comp.IconTexture = _resCache.GetResource<TextureResource>(ent.Comp.InteractionIconPath).Texture;
    }

    public void HandleUse(EntityUid entity, bool isDown)
    {
        if(!TryComp<InteractionComponent>(entity, out var interactionComponent) || !interactionComponent.IsEnabled || !isDown || interactionComponent.CurrentInteractible is null) return;
        
        foreach (var action in interactionComponent.CurrentInteractible.Value.Item1.Actions)
        {
            action.Act(IoCManager.Instance!);
        }
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<InteractionComponent, TransformComponent>();

        while (query.MoveNext(out var interaction, out var transform))
        {
            float distance((InteractibleComponent, TransformComponent) a)
            {
                return (transform.LocalPosition - a.Item2.LocalPosition).Length();
            }
            
            interaction.CurrentInteractible = 
                EntityQuery<InteractibleComponent, TransformComponent>()
                .OrderBy(distance)
                .Where(a => distance(a) < interaction.MaxDistance).FirstOrNull();
        }
    }
}

public sealed class UseInretactionCommand : InputCmdHandler
{
    private readonly InteractionSystem _interactionSystem;

    public UseInretactionCommand(InteractionSystem interactionSystem)
    {
        _interactionSystem = interactionSystem;
    }

    public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
    {
        if (session?.AttachedEntity is null) return false;

        _interactionSystem.HandleUse(session.AttachedEntity.Value, message.State == BoundKeyState.Down);

        return false;
    }
}
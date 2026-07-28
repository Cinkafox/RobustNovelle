using System.Linq;
using Content.Client.Dialog.Components;
using Content.Client.Interaction.Components;
using Content.Client.Scene.Systems;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Client.Interaction.Systems;

public sealed partial class InteractionSystem : EntitySystem
{
    [Dependency] private IOverlayManager _overlayManager = default!;
    [Dependency] private SceneSystem _sceneSystem = default!;

    public override void Initialize()
    {
        _overlayManager.AddOverlay(new InteractionOverlay());

        CommandBinds.Builder
            .Bind(EngineKeyFunctions.Use, new UseInretactionCommand(this))
            .Register<InteractionSystem>();
    }

    public void HandleUse(EntityUid entity)
    {
        if (!TryComp<InteractionComponent>(entity, out var interactionComponent) ||
            !interactionComponent.IsEnabled ||
            interactionComponent.CurrentInteractible is null
           ) return;
        
        _sceneSystem.LoadScene(entity, interactionComponent.CurrentInteractible.Value.Item1.Scene);
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<InteractionComponent, TransformComponent>();

        while (query.MoveNext(out var uid, out var interaction, out var transform))
        {
            float distance((InteractibleComponent, TransformComponent) a)
            {
                return (transform.LocalPosition - a.Item2.LocalPosition).Length();
            }

            interaction.CurrentInteractible =
                EntityQuery<InteractibleComponent, TransformComponent>()
                    .OrderBy(distance)
                    .Where(a => transform.MapID == a.Item2.MapID &&
                                distance(a) < a.Item1.MaxDistance).FirstOrNull();

            if (interaction.CurrentInteractible is { Item1.InvokeImmediately: true }) HandleUse(uid);
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

    public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session,
        IFullInputCmdMessage message)
    {
        if (session?.AttachedEntity is null || message.State != BoundKeyState.Down) return false;

        _interactionSystem.HandleUse(session.AttachedEntity.Value);

        return false;
    }
}
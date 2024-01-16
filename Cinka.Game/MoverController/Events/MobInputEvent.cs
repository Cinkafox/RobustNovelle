using Cinka.Game.MoverController.Components;
using Robust.Shared.GameObjects;

namespace Cinka.Game.MoverController.Events;

/// <summary>
/// Raised on an entity whenever it has a movement input change.
/// </summary>
[ByRefEvent]
public readonly struct MoveInputEvent
{
    public readonly EntityUid Entity;
    public readonly InputMoverComponent Component;
    public readonly MoveButtons OldMovement;

    public MoveInputEvent(EntityUid entity, InputMoverComponent component, MoveButtons oldMovement)
    {
        Entity = entity;
        Component = component;
        OldMovement = oldMovement;
    }
}
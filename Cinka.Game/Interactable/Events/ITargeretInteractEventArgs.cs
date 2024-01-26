using Robust.Shared.GameObjects;

namespace Cinka.Game.Interactable.Events;

public interface ITargetedInteractEventArgs
{
    /// <summary>
    /// Performer of the attack
    /// </summary>
    EntityUid User { get; }
    /// <summary>
    /// Target of the attack
    /// </summary>
    EntityUid Target { get; }

}
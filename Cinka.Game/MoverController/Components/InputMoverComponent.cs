using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Cinka.Game.MoverController.Components;

[RegisterComponent]
public sealed partial class InputMoverComponent : Component
{
    /// <summary>
    /// Should our velocity be applied to our parent?
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("toParent")]
    public bool ToParent = false;

    public GameTick LastInputTick;
    public ushort LastInputSubTick;

    public Vector2 CurTickWalkMovement;
    public Vector2 CurTickSprintMovement;

    public MoveButtons HeldMoveButtons = MoveButtons.None;

    /// <summary>
    /// Entity our movement is relative to.
    /// </summary>
    public EntityUid? RelativeEntity;

    /// <summary>
    /// Although our movement might be relative to a particular entity we may have an additional relative rotation
    /// e.g. if we've snapped to a different cardinal direction
    /// </summary>
    [ViewVariables]
    public Angle TargetRelativeRotation = Angle.Zero;

    /// <summary>
    /// The current relative rotation. This will lerp towards the <see cref="TargetRelativeRotation"/>.
    /// </summary>
    [ViewVariables]
    public Angle RelativeRotation;

    /// <summary>
    /// If we traverse on / off a grid then set a timer to update our relative inputs.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan LerpTarget;

    public const float LerpTime = 1.0f;

    public bool Sprinting => (HeldMoveButtons & MoveButtons.Walk) == 0x0;

    [ViewVariables(VVAccess.ReadWrite)]
    public bool CanMove = true;
}
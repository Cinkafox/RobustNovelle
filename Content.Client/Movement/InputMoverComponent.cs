using System.Numerics;
using Content.Client.Utils;

namespace Content.Client.Movement;

[RegisterComponent]
public sealed partial class InputMoverComponent : Component
{
    [DataField] public float BaseSpeed = 3f;
    [DataField] public float RunningSpeed = 4f;
    [DataField] public DirectionFlag Direction;
    
    [DataField] public bool IsMoveEnabled;
    [DataField] public bool IsRunning;
    
    [ViewVariables] public float OldMagnitude = 0f;
    
    [ViewVariables(VVAccess.ReadOnly)] 
    public bool IsMoving => Direction != DirectionFlag.None;
    
    [ViewVariables(VVAccess.ReadOnly)]
    public float Magnitude => IsMoveEnabled ? (IsRunning ? RunningSpeed : BaseSpeed) : 0;
    
    [ViewVariables(VVAccess.ReadOnly)]
    public Vector2 MoveVelocity => Direction.ToVec() * Magnitude;
    
}
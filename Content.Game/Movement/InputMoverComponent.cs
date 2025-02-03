using System.Numerics;

namespace Content.Game.Movement;

[RegisterComponent]
public sealed partial class InputMoverComponent : Component
{
    [DataField] public bool IsRunning;
    [DataField] public Direction Direction;
    [DataField] public float Speed;
    public int ButtonPressed;
    public bool IsMoving => ButtonPressed > 0;
}
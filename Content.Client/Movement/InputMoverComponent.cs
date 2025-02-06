using System.Numerics;

namespace Content.Client.Movement;

[RegisterComponent]
public sealed partial class InputMoverComponent : Component
{
    [DataField] public bool IsRunning;
    [DataField] public bool IsEnabled;
    [DataField] public Direction Direction;
    [DataField] public float Speed;
    public int ButtonPressed;
    public bool IsMoving => ButtonPressed > 0;
}
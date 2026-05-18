namespace Content.Client.Movement;

[RegisterComponent]
public sealed partial class InputMoverComponent : Component
{
    [ViewVariables] public int ButtonPressed;
    [DataField] public Direction Direction;
    [DataField] public bool IsEnabled;
    [DataField] public bool IsRunning;
    [DataField] public float Speed;
    [ViewVariables(VVAccess.ReadOnly)] public bool IsMoving => ButtonPressed > 0;
}
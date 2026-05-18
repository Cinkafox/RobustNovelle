namespace Content.Client.Camera.Components;

[RegisterComponent]
public sealed partial class CameraComponent : Component
{
    [ViewVariables] public bool FirstTimeInMap = false;
    [ViewVariables] public EntityUid? FollowUid;
}
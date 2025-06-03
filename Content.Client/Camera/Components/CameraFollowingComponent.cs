namespace Content.Client.Camera.Components;

[RegisterComponent]
public sealed partial class CameraFollowingComponent : Component
{
    [ViewVariables] public EntityUid CameraUid;
}
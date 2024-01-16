using Robust.Shared.GameObjects;

namespace Cinka.Game.Camera;

[RegisterComponent]
public sealed partial class CameraControlledComponent : Component
{
    public EntityUid CameraUid;
}
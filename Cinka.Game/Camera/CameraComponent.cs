using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Cinka.Game.Camera;

[RegisterComponent]
public sealed partial class CameraComponent : Component
{
    [ViewVariables] public EntityUid AttachedEntity;
}
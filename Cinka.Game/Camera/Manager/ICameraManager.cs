using Robust.Shared.GameObjects;

namespace Cinka.Game.Camera.Manager;

public interface ICameraManager
{
    public void Initialize();
    public void AttachEntity(EntityUid uid);
    public EntityUid GetCameraEntity();
}
using Cinka.Game.Camera.Manager;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Cinka.Game.Camera;

public sealed class CameraSystem : EntitySystem
{
    [Dependency] private readonly TransformSystem _transform = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<CameraComponent,CameraAttachedToEntityEvent>(OnAttach);
    }

    private void OnAttach(EntityUid uid, CameraComponent component, CameraAttachedToEntityEvent args)
    {
        _transform.SetParent(uid,component.AttachedEntity);
        EnsureComp<CameraControlledComponent>(component.AttachedEntity).CameraUid = uid;
    }
}
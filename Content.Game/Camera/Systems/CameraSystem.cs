using Content.Game.Location.Systems;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Map;

namespace Content.Game.Camera.Systems;

public sealed class CameraSystem : EntitySystem
{
    public static string CameraProtoName = "Camera";
    
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;

    private Entity<TransformComponent>? _cameraUid;
    private Entity<TransformComponent>? _followUid;
    
    public void FollowTo(EntityUid entityUid)
    {
        var entTransform = Transform(entityUid);

        if (_cameraUid is null)
        {
            var uid = _entityManager.SpawnEntity(CameraProtoName,
                MapCoordinates.Nullspace);
            _cameraUid = new Entity<TransformComponent>(uid, Transform(uid));

            _playerManager.SetAttachedEntity(_playerManager.LocalSession, _cameraUid);
        }

        _followUid = new Entity<TransformComponent>(entityUid, entTransform);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        
        if(_cameraUid is null || _followUid is null) return;

        if (!_cameraUid.Value.Comp.ParentUid.IsValid())
        {
            _transformSystem.SetParent(_cameraUid.Value, _followUid.Value);
        }
        else if (_cameraUid.Value.Comp.ParentUid != _followUid.Value.Comp.ParentUid)
        {
            _transformSystem.SetParent(_cameraUid.Value, _followUid.Value.Comp.ParentUid);
        }

        var delta = _cameraUid.Value.Comp.LocalPosition - _followUid.Value.Comp.LocalPosition;
        _cameraUid.Value.Comp.LocalPosition += delta / 2;
    }
}
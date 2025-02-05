using System.Numerics;
using Content.Game.Camera.Components;
using Content.Game.Location.Systems;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Game.Camera.Systems;

public sealed class CameraSystem : EntitySystem
{
    public static string CameraProtoName = "Camera";
    
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;
    [Dependency] private readonly EyeSystem _eyeSystem = default!;
    [Dependency] private readonly LocationSystem _location = default!;

    public Entity<TransformComponent,CameraComponent>? CameraUid { get; private set; }
    public EntProtoId? CurrentFollowEnt;
    
    public bool ResetFollowing()
    {
        if(CurrentFollowEnt is not null)
        {
            FollowTo(CurrentFollowEnt.Value);
            return true;
        }

        return false;
    }
    public void FollowTo(EntProtoId id)
    {
        if (_location.TryGetLocationEntity(id, out var camFol))
            FollowTo(camFol);
        
        CurrentFollowEnt = id;
    }
    
    public void FollowTo(EntityUid entityUid)
    {
        var entTransform = Transform(entityUid);

        if (CameraUid is null)
        {
            var uid = _entityManager.SpawnEntity(CameraProtoName,
                MapCoordinates.Nullspace);
            var camComp = EnsureComp<CameraComponent>(uid);
            CameraUid = new Entity<TransformComponent,CameraComponent>(uid, Transform(uid),camComp);

            _playerManager.SetAttachedEntity(_playerManager.LocalSession, CameraUid);
        }

        CameraUid.Value.Comp2.FollowUid = new Entity<TransformComponent>(entityUid, entTransform);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<TransformComponent, CameraComponent, EyeComponent>();
        while (query.MoveNext(out var camUid,out var transformComponent, out var cameraComponent, out var eyeComponent))
        {
            _eyeSystem.SetZoom(camUid, new Vector2(0.5f),eyeComponent);
            
            var followUid = cameraComponent.FollowUid;
            if(followUid is null) continue;
            
            if (!transformComponent.ParentUid.IsValid())
            {
                _transformSystem.SetParent(camUid, followUid.Value);
            }
            else if (transformComponent.ParentUid != followUid.Value.Comp.ParentUid)
            {
                _transformSystem.SetParent(camUid, followUid.Value.Comp.ParentUid);
            }
            
            var delta = transformComponent.LocalPosition - followUid.Value.Comp.LocalPosition;
            transformComponent.LocalPosition -= delta / 2;
        }
    }
}
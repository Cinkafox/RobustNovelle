using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Client.Camera.Components;
using Content.Client.Location.Systems;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.Camera.Systems;

public sealed class CameraSystem : EntitySystem
{
    public static readonly string CameraProtoName = "Camera";
    
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;

    public void FollowTo(Entity<CameraComponent> cameraUid, EntityUid entityUid)
    {
        if (TryComp<MapComponent>(entityUid, out _))
        {
            _transformSystem.SetParent(cameraUid, entityUid);
            cameraUid.Comp.FirstTimeInMap = true;
            return;
        }

        // Clean some thinks after self
        if (TryComp<CameraFollowingComponent>(cameraUid.Comp.FollowUid, out var cameraFollowingComponent) &&
            cameraFollowingComponent.CameraUid == cameraUid.Owner)
            RemComp(cameraUid.Comp.FollowUid.Value, cameraFollowingComponent);
        
        var followTransform = Transform(entityUid); 
        var transformComponent = Transform(cameraUid);
            
        if (transformComponent.ParentUid != followTransform.ParentUid)
        {
            _transformSystem.SetParent(cameraUid, followTransform.ParentUid);
        }

        if (cameraUid.Comp.FirstTimeInMap)
        {
            _transformSystem.SetLocalPosition(cameraUid, Transform(entityUid).LocalPosition);
            cameraUid.Comp.FirstTimeInMap = false;
        }
        
        cameraUid.Comp.FollowUid = entityUid;
        EnsureComp<CameraFollowingComponent>(entityUid).CameraUid = cameraUid;
        
        Log.Debug($"Following to {Name(entityUid)}");
    }

    public Entity<CameraComponent> CreateCamera(ICommonSession session)
    {
        var uid = _entityManager.SpawnEntity(CameraProtoName,
            MapCoordinates.Nullspace);
        var camComp = EnsureComp<CameraComponent>(uid);
        var cameraUid = new Entity<CameraComponent>(uid,camComp);

        _playerManager.SetAttachedEntity(session, cameraUid);
        return cameraUid;
    }

    public bool TryGetCamera(EntityUid? entityUid,[NotNullWhen(true)] out Entity<CameraComponent>? cameraUid)
    {
        cameraUid = null;
        if (!TryComp<CameraFollowingComponent>(entityUid, out var cameraFollowingComponent) || 
            !TryComp<CameraComponent>(cameraFollowingComponent.CameraUid, out var cameraComponent)) 
            return false;

        cameraUid = new Entity<CameraComponent>(cameraFollowingComponent.CameraUid, cameraComponent);
        return true;
    }

    public bool TryGetCameraSession(EntityUid? entityUid,[NotNullWhen(true)] out ICommonSession? session)
    {
        session = null;
        if (!TryComp<CameraFollowingComponent>(entityUid, out var cameraFollowingComponent) || 
            !_playerManager.TryGetSessionByEntity(cameraFollowingComponent.CameraUid, out session)) 
            return false;

        return true;
    }

    public override void FrameUpdate(float frameTime)
    {
        base.FrameUpdate(frameTime);

        var query = EntityQueryEnumerator<TransformComponent, CameraComponent>();
        while (query.MoveNext(out var camUid,out var transformComponent, out var cameraComponent))
        {
            var followUid = cameraComponent.FollowUid;
            if(followUid is null) continue;

            var followTransform = Transform(followUid.Value);
            
            var delta = transformComponent.LocalPosition - followTransform.LocalPosition;
            _transformSystem.SetLocalPosition(camUid, transformComponent.LocalPosition - delta / 20);
        }
    }
}
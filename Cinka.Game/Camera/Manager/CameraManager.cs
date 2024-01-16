using System;
using Cinka.Game.Location.Managers;
using Robust.Client;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;

namespace Cinka.Game.Camera.Manager;

public sealed class CameraManager : ICameraManager
{
    public static string CameraProtoName = "Camera";
    [Dependency] private readonly IBaseClient _client = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly ILocationManager _locationManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    public EntityUid CameraUid;

    public void Initialize()
    {
        IoCManager.InjectDependencies(this);

        CameraUid = _entityManager.SpawnEntity(CameraProtoName,
            new MapCoordinates(0, 0, _locationManager.GetCurrentLocationId()));
        var camcomp = _entityManager.EnsureComponent<CameraComponent>(CameraUid);
        if (_playerManager.LocalSession == null) throw new Exception("PUK SRINK PLAYER IS NULL!");

        _playerManager.SetAttachedEntity(_playerManager.LocalSession, CameraUid);
    }

    public void AttachEntity(EntityUid uid)
    {
        Logger.Debug($"Camera attached to {uid}");
        _entityManager.GetComponent<CameraComponent>(CameraUid).AttachedEntity = uid;
        _entityManager.EventBus.RaiseLocalEvent(CameraUid,new CameraAttachedToEntityEvent());
    }

    public EntityUid GetCameraEntity()
    {
        return CameraUid;
    }
}

public sealed class CameraAttachedToEntityEvent : EntityEventArgs
{
    
}
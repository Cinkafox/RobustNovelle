using System;
using Cinka.Game.Location.Managers;
using Robust.Client;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;

namespace Cinka.Game.Camera.Manager;

public sealed class CameraManager: ICameraManager
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly ILocationManager _locationManager = default!;
    [Dependency] private readonly IBaseClient _client = default!;

    public static string CameraProtoName = "Camera";
    
    public void Initialize()
    {
        IoCManager.InjectDependencies(this);
        
        var entity = _entityManager.SpawnEntity(CameraProtoName, new MapCoordinates(0, 0,_locationManager.GetCurrentLocationId()));
        if (_playerManager.LocalPlayer == null)
        {
            throw new Exception("PUK SRINK PLAYER IS NULL!");
        }
        _playerManager.LocalPlayer.AttachEntity(entity,_entityManager,_client);
    }
}
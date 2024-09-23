using System;
using Content.Game.Location.Managers;
using Robust.Client;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Game.Camera.Manager;

public sealed class CameraManager : ICameraManager
{
    public static string CameraProtoName = "Camera";
    
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly ILocationManager _locationManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    public void Initialize()
    {
        IoCManager.InjectDependencies(this);

        var entity = _entityManager.SpawnEntity(CameraProtoName,
            new MapCoordinates(0, 0, _locationManager.GetCurrentLocationId()));

        _playerManager.SetAttachedEntity(_playerManager.LocalSession, entity);
    }
}
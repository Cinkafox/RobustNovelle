using System.Collections.Generic;
using Cinka.Game.Background.Manager;
using Cinka.Game.Location.Data;
using Cinka.Game.Parallax.Managers;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Prototypes;

namespace Cinka.Game.Location.Managers;

public sealed class LocationManager : ILocationManager
{
    [Dependency] private readonly IBackgroundManager _backgroundManager = default!;
    [Dependency] private readonly IParallaxManager _parallaxManager = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    //[Dependency] private readonly MapSystem _mapSystem = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    private MapId _currentLocationId;
    private readonly Dictionary<string, LocationPrototype> _locationPrototypes = new();

    private readonly Dictionary<string, MapId> _locationsId = new();

    public void Initialize()
    {
        IoCManager.InjectDependencies(this);
        LoadLocation("default");
    }


    public MapId GetCurrentLocationId()
    {
        return _currentLocationId;
    }

    public void LoadLocation(string prototype)
    {
        LoadLocation(prototype, true);
    }

    private bool TryInitializeLocation(string prototype)
    {
        if (!_prototypeManager.TryIndex<LocationPrototype>(prototype, out var prot))
            return false;

        var mapId = _mapManager.CreateMap();
        var mapEnt = _mapManager.GetMapEntityId(mapId);
        _entityManager.EnsureComponent<PhysicsMapComponent>(mapEnt);
        Logger.Debug($"Current location ID: {mapId}");
        _locationsId.TryAdd(prototype, mapId);
        _locationPrototypes.TryAdd(prototype, prot);
        
        return true;
    }

    public void LoadLocation(string prototype, bool init)
    {
        if (!_locationsId.TryGetValue(prototype, out var mapId))
        {
            if (!init) return;
            TryInitializeLocation(prototype);
            LoadLocation(prototype, false);
            return;
        }
        
        _parallaxManager.LoadParallaxByName(_locationPrototypes[prototype].Parallax);
        _currentLocationId = mapId;
    }
}
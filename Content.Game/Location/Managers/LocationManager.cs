using System.Collections.Generic;
using Content.Game.Background;
using Content.Game.Location.Data;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Game.Location.Managers;

public sealed class LocationManager : ILocationManager
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
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

    private bool TryInitializeLocation(string prototype)
    {
        if (!_prototypeManager.TryIndex<LocationPrototype>(prototype, out var prot))
            return false;

        var mapId = _mapManager.CreateMap();
        Logger.Debug($"Current location ID: {mapId}");
        _locationsId.TryAdd(prototype, mapId);
        _locationPrototypes.TryAdd(prototype, prot);

        return true;
    }

    public void LoadLocation(string prototype)
    {
        if (!_locationsId.TryGetValue(prototype, out var mapId))
        {
            TryInitializeLocation(prototype);
            LoadLocation(prototype);
            return;
        }

        _entityManager.System<BackgroundSystem>().LoadBackground(_locationPrototypes[prototype].Background);
        _currentLocationId = mapId;
    }
}
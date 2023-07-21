using System.Collections.Generic;
using Cinka.Game.Background.Manager;
using Cinka.Game.Location.Data;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Cinka.Game.Location.Managers;

public sealed class LocationManager : ILocationManager
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly IBackgroundManager _backgroundManager = default!;

    private Dictionary<string,MapId> _locationsId = new ();
    private Dictionary<string, LocationPrototype> _locationPrototypes = new();
    private MapId _currentLocationId;

    private bool TryInitializeLocation(string prototype)
    {
        if(!_prototypeManager.TryIndex<LocationPrototype>(prototype, out var prot))
            return false;
        
        var mapId = _mapManager.CreateMap();
        Logger.Debug($"Current location ID: {mapId}");
        _locationsId.TryAdd(prototype,mapId);
        _locationPrototypes.TryAdd(prototype,prot);
        
        return true;
    }
    
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
        LoadLocation(prototype,true);
    }

    public void LoadLocation(string prototype,bool init)
    {
        if (!_locationsId.TryGetValue(prototype, out var mapId))
        {
            if (!init) return;
            TryInitializeLocation(prototype);
            LoadLocation(prototype,false);
            return;
        }
        _backgroundManager.LoadBackground(_locationPrototypes[prototype].BackgroundPrototype);
        _currentLocationId = mapId;
    }
}
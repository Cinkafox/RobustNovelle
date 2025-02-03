using Content.Game.Background;
using Content.Game.Location.Components;
using Content.Game.Location.Data;
using Robust.Shared.Prototypes;

namespace Content.Game.Location.Systems;

public sealed class LocationSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    
    private EntityUid _currentLocationId;
    private readonly Dictionary<string, LocationPrototype> _locationPrototypes = new();

    private readonly Dictionary<string, EntityUid> _locationsId = new();
    
    public EntityUid GetCurrentLocationId()
    {
        return _currentLocationId;
    }
    
    private bool TryInitializeLocation(string prototype, out EntityUid mapUid)
    {
        mapUid = EntityUid.Invalid;
        if (!_prototypeManager.TryIndex<LocationPrototype>(prototype, out var prot))
        {
            Logger.Error($"PROTO LOCATION {prototype} NOT EXIST!!!");
            return false;
        }

        mapUid = _mapSystem.CreateMap();
        Logger.Debug($"Current location ID: {mapUid}");
        _locationsId.TryAdd(prototype, mapUid);
        _locationPrototypes.TryAdd(prototype, prot);

        return true;
    }

    public EntityUid LoadLocation(string prototype)
    {
        if (!_locationsId.TryGetValue(prototype, out var mapId) &&
            !TryInitializeLocation(prototype, out mapId))
        {
            throw new Exception("Увы...");
        }

        var proto = _locationPrototypes[prototype];

        if (proto.Background is not null)
        {
            _entityManager.System<BackgroundSystem>().LoadBackground(proto.Background);
        }
        
        else if (proto.Location is not null)
        {
            var loc = AddComp<LocationComponent>(mapId);
            loc.CurrentLocation = proto.Location;
        }
        
        _currentLocationId = mapId;
        return _currentLocationId;
    }
}
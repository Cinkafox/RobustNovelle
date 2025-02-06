using System.Collections;
using System.IO;
using System.Linq;
using System.Numerics;
using Content.Client.Background;
using Content.Client.Location.Components;
using Content.Client.Location.Data;
using Robust.Client.GameStates;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Client.Location.Systems;

public sealed class LocationSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IResourceManager _resourceManager = default!;
    
    private EntityUid _currentLocationId;
    private readonly Dictionary<string, LocationPrototype> _locationPrototypes = new();

    private readonly Dictionary<string, EntityUid> _locationsId = new();
    private readonly Dictionary<EntityUid, Dictionary<EntProtoId,EntityUid>> _entities = new();

    private EntProtoId WallsId = "Wall";

    public EntityUid GetCurrentLocationId()
    {
        return _currentLocationId;
    }
    
    private bool TryInitializeLocation(string prototype, out EntityUid mapUid)
    {
        mapUid = EntityUid.Invalid;
        if (!_prototypeManager.TryIndex<LocationPrototype>(prototype, out var proto))
        {
            Logger.Error($"PROTO LOCATION {prototype} NOT EXIST!!!");
            return false;
        }

        mapUid = _mapSystem.CreateMap();
        Logger.Debug($"Current location ID: {mapUid}");
        _locationsId.TryAdd(prototype, mapUid);
        _locationPrototypes.TryAdd(prototype, proto);
        var hashEnt = new Dictionary<EntProtoId, EntityUid>();
        _entities.Add(mapUid,hashEnt);
        
        if (proto.Location is not null)
        {
            var loc = AddComp<LocationComponent>(mapUid);
            loc.CurrentLocation = proto.Location;

            if (proto.Location.Map is not null)
            {
                using var sr =  _resourceManager.ContentFileReadText(proto.Location.Map.Value);
                var map = new ColliderMap(sr);
                foreach (var pos in map)
                {
                    Spawn(WallsId, new EntityCoordinates(mapUid, pos));
                }
            }
        }
        
        if (proto.Entities is { } entities)
        {
            foreach (var entity in entities)
            {
                var uid = Spawn(entity.Entity, new EntityCoordinates(mapUid, entity.Position));
                Logger.Debug("SPAWN " + uid + " " + entity.Entity);
                hashEnt.Add(entity.Entity, uid);
            }
        }
        
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
            _entityManager.System<BackgroundSystem>().LoadBackground(proto.Background.Value);
        }
        
        _currentLocationId = mapId;
        return _currentLocationId;
    }

    public bool TryGetLocationEntity(EntProtoId? ent, out EntityUid uid)
    {
        uid = EntityUid.Invalid;
        return ent is not null &&
               _entities.TryGetValue(_currentLocationId, out var dictionary) && 
               dictionary.TryGetValue(ent.Value, out uid);
    }

    public IEnumerable<EntityUid> GetLocationEnumerator()
    {
        if(!_entities.TryGetValue(_currentLocationId, out var entityUids)) return [];
        return entityUids.Values.Select(a => a);
    }
}

public sealed class ColliderMap: IEnumerable<Vector2i>
{
    private readonly Dictionary<Vector2i, bool> _map = new();
    public int Colon { get; private set; }
    public int Row { get; private set; }
    
    public ColliderMap(StreamReader streamReader)
    {
        string? line;
        while ((line = streamReader.ReadLine()) != null)
        {
            for (Row = 0; Row < line.Length; Row++)
            {
                _map.Add(new Vector2i(Row, Colon), line[Row] == '#');
            }

            Colon++;
        }
    }

    public IEnumerator<Vector2i> GetEnumerator()
    {
        foreach (var (pos, isEnabled) in _map)
        {
            if (isEnabled) yield return pos;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
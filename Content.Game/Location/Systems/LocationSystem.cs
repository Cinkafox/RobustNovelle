using System.Collections;
using System.IO;
using System.Numerics;
using Content.Game.Background;
using Content.Game.Location.Components;
using Content.Game.Location.Data;
using Robust.Client.GameStates;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Game.Location.Systems;

public sealed class LocationSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IResourceManager _resourceManager = default!;
    
    private EntityUid _currentLocationId;
    private readonly Dictionary<string, LocationPrototype> _locationPrototypes = new();

    private readonly Dictionary<string, EntityUid> _locationsId = new();

    private EntProtoId WallsId = "Wall";

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

            if (proto.Location.Map is not null)
            {
               using var sr =  _resourceManager.ContentFileReadText(proto.Location.Map.Value);
               var map = new ColliderMap(sr);
               foreach (var pos in map)
               {
                   Spawn(WallsId, new EntityCoordinates(mapId, pos));
               }
            }
        }
        
        _currentLocationId = mapId;
        return _currentLocationId;
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
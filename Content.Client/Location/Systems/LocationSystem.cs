using System.Collections;
using System.IO;
using System.Linq;
using System.Numerics;
using Content.Client.Background;
using Content.Client.Location.Components;
using Content.Client.Location.Data;
using Robust.Client.Graphics;
using Robust.Client.Utility;
using Robust.Shared.ContentPack;
using Robust.Shared.Map;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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

    private readonly EntProtoId _wallsId = "Wall";

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
            
            proto.Location.Map ??= new ResPath(proto.Location.Path.ToString().Replace(".png",".map.png"));
            
            using var stream = _resourceManager.ContentFileRead(proto.Location.Map.Value.ToString());
            var texture = Image.Load<Rgba32>(stream);
            var map = new ColliderMap(texture);
            foreach (var pos in map)
            {
                Spawn(_wallsId, new EntityCoordinates(mapUid, pos - new Vector2(-0.5f, 0.5f)));
            }
        }
        
        if (proto.Entities is { } entities)
        {
            foreach (var entity in entities)
            {
                var uid = Spawn(entity.Entity, new EntityCoordinates(mapUid, entity.Position));
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
    public int Width { get; private set; }
    public int Height { get; private set; }

    private List<Edge> edges = default!;
    
    public ColliderMap(Image<Rgba32> image)
    {
        Width = image.Width;
        Height = image.Height;
        
        var span = image.GetPixelSpan();
        
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var imgPixel = span[Width*y + x];
                _map.Add(new Vector2i(x, Height - y), imgPixel.A != 0);
            }
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
    
    public List<Vector2> ToVectors()
    {
        DetectEdges();
        var edg = edges.ToList();
        var l = new List<Vector2>();
        var a = edg.First();

        do
        {
            l.Add(a.Start);
            edg.Remove(a);
        } 
        while (edg.TryFirstOrDefault(pr => pr.Start == a.End, out a));
            
           
        return l;
    } 
    
    private void DetectEdges()
    {
        edges = new List<Edge>();
            
        foreach (var vector2I in _map.Where(kvp => kvp.Value).Select(kvp => kvp.Key))
        {
            // Проверка соседей
            var neighbors = new[]
            {
                new Vector2i(vector2I.X, vector2I.Y - 1), // Вверх
                new Vector2i(vector2I.X + 1, vector2I.Y), // Вправо
                new Vector2i(vector2I.X, vector2I.Y + 1), // Вниз
                new Vector2i(vector2I.X - 1, vector2I.Y)  // Влево
            };

            // Добавляем грани, где нет соседей
            if (!_map.ContainsKey(neighbors[0])) 
                edges.Add(new Edge(
                    new Vector2i(vector2I.X, vector2I.Y),
                    new Vector2i(vector2I.X + 1, vector2I.Y)
                ));

            if (!_map.ContainsKey(neighbors[1])) 
                edges.Add(new Edge(
                    new Vector2i(vector2I.X + 1, vector2I.Y),
                    new Vector2i(vector2I.X + 1, vector2I.Y + 1)
                ));

            if (!_map.ContainsKey(neighbors[2])) 
                edges.Add(new Edge(
                    new Vector2i(vector2I.X + 1, vector2I.Y + 1),
                    new Vector2i(vector2I.X, vector2I.Y + 1)
                ));

            if (!_map.ContainsKey(neighbors[3])) 
                edges.Add(new Edge(
                    new Vector2i(vector2I.X, vector2I.Y + 1),
                    new Vector2i(vector2I.X, vector2I.Y)
                ));
        }

        // Удаление дубликатов (если две соседние ячейки отсутствуют)
        edges = edges
            .GroupBy(e => new { Start = e.Start, End = e.End })
            .Select(g => g.First())
            .ToList();
    }

    public PolygonShape ToShape()
    {
        var shape = new PolygonShape();
        shape.Set(ToVectors());
        return shape;
    }
}

public sealed class Edge
{
    public Vector2i Start { get; }
    public Vector2i End { get; }

    public Edge(Vector2i start, Vector2i end)
    {
        Start = start;
        End = end;
    }
}
using System.Collections;
using System.IO;
using System.Linq;
using System.Numerics;
using Content.Client.Audio.Systems;
using Content.Client.Background;
using Content.Client.Location.Components;
using Content.Client.Location.Data;
using Robust.Client.Audio;
using Robust.Client.Utility;
using Robust.Shared.Audio;
using Robust.Shared.ContentPack;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client.Location.Systems;

public sealed class LocationSystem : EntitySystem
{
    [Dependency] private readonly AudioSystem _audioSystem = default!;
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IResourceManager _resourceManager = default!;
    [Dependency] private readonly BackgroundSystem _backgroundSystem = default!;

    private readonly Dictionary<string, EntityUid> _locationsId = new();

    private readonly EntProtoId _wallsId = "Wall";
    
    private bool TryInitializeLocation(LocationPrototype proto, out EntityUid mapUid)
    {
        mapUid = _mapSystem.CreateMap(out var mapId);
        Log.Info($"Current location ID: {mapUid}");
        _locationsId.TryAdd(proto.ID, mapUid);
        var loc = AddComp<LocationComponent>(mapUid);
        
        if (proto.Location is not null)
        {
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
                var uid = Spawn(entity.Entity, new EntityCoordinates(mapUid, entity.Position - new Vector2(-0.5f, 0.5f)));
                loc.EntityDefinitions.Add(entity.Entity, uid);
            }
        }
        
        foreach (var sound in proto.AmbientSounds)
        {
            loc.Ambients.Add(
                _audioSystem.PlayEntity(sound, Filter.BroadcastMap(mapId), mapUid, false, AudioParams.Default.WithVolume(0.5f).WithLoop(true))!.Value.Entity);
        }
        
        return true;
    }

    public EntityUid LoadLocation(string prototype)
    {
        if (!_prototypeManager.TryIndex<LocationPrototype>(prototype, out var proto))
        {
            throw new Exception($"PROTO LOCATION {prototype} NOT EXIST!!!");
        }
        
        if (!_locationsId.TryGetValue(prototype, out var mapId) &&
            !TryInitializeLocation(proto, out mapId))
        {
            throw new Exception("Увы...");
        }

        if (proto.Location is not null)
        {
            _backgroundSystem.LoadBackground(mapId, null);
        }
        
        if (proto.Background is not null)
        {
            _backgroundSystem.LoadBackground(mapId, proto.Background.Value);
        }
        
        return mapId;
    }

    public bool TryGetLocationEntity(EntityUid anotherLocationEntity, EntProtoId? ent, out EntityUid uid)
    {
        uid = EntityUid.Invalid;
        return ent is not null && 
               TryComp<LocationComponent>(GetMapFromEntity(anotherLocationEntity), out var component) &&
               component.EntityDefinitions.TryGetValue(ent.Value, out uid);
    }

    public IEnumerable<EntityUid> GetLocationEnumerator(EntityUid anotherLocationEntity)
    {
        if(!TryComp<LocationComponent>(GetMapFromEntity(anotherLocationEntity), out var component)) return [];
        return component.EntityDefinitions.Values;
    }

    private EntityUid GetMapFromEntity(EntityUid entity)
    {
        if (HasComp<MapComponent>(entity)) return entity;
        return GetMapFromEntity(Transform(entity).ParentUid);
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
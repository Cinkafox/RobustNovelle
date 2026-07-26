using System.Collections;
using System.Linq;
using System.Numerics;
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

public sealed partial class LocationSystem : EntitySystem
{
    private readonly Dictionary<string, EntityUid> _locationsId = new();
    
    [Dependency] private AudioSystem _audioSystem = default!;
    [Dependency] private SharedMapSystem _mapSystem = default!;
    [Dependency] private IPrototypeManager _prototypeManager = default!;
    [Dependency] private IResourceManager _resourceManager = default!;

    private void InitializeLocation(ProtoId<LocationPrototype> prototype, out EntityUid mapUid)
    {
        if (!_prototypeManager.TryIndex(prototype, out var proto))
            throw new Exception($"PROTO LOCATION {prototype} NOT EXIST!!!");
        
        mapUid = _mapSystem.CreateMap(out var mapId);
        Log.Info($"Current location ID: {mapUid}");
        _locationsId.TryAdd(proto.ID, mapUid);
        var loc = AddComp<LocationComponent>(mapUid);
        
        proto.Location.LoadLocation(new Entity<LocationComponent>(mapUid, loc), EntityManager, this, _resourceManager);
        
        foreach (var entity in proto.Entities)
        {
            var uid = Spawn(entity.Entity,
                new EntityCoordinates(mapUid, entity.Position - new Vector2(-0.5f, 0.5f)));
            loc.EntityDefinitions.Add(entity.Entity, uid);
        }
        
        foreach (var sound in proto.AmbientSounds)
            loc.Ambients.Add(
                _audioSystem.PlayEntity(sound, Filter.BroadcastMap(mapId), mapUid, false,
                    AudioParams.Default.WithVolume(0.5f).WithLoop(true))!.Value.Entity);
        
        loc.CurrentLocation = proto.Location;
    }

    public EntityUid LoadLocation(ProtoId<LocationPrototype> prototype)
    {
        EntityUid mapUid;
        LocationComponent? component;

        if (_locationsId.TryGetValue(prototype, out mapUid) && TryComp(mapUid, out component))
        {
            component.CurrentLocation.OnExit(new Entity<LocationComponent>(mapUid, component), EntityManager, this, _resourceManager);
        }
        else
        {
            InitializeLocation(prototype, out mapUid);
            component = Comp<LocationComponent>(mapUid);
        }
        
        component.CurrentLocation.OnEnter(new Entity<LocationComponent>(mapUid, component), EntityManager, this, _resourceManager);

        return mapUid;
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
        if (!TryComp<LocationComponent>(GetMapFromEntity(anotherLocationEntity), out var component)) return [];
        return component.EntityDefinitions.Values;
    }

    private EntityUid GetMapFromEntity(EntityUid entity)
    {
        if (HasComp<MapComponent>(entity)) return entity;
        return GetMapFromEntity(Transform(entity).ParentUid);
    }
}

public sealed class ColliderMap : IEnumerable<Vector2i>
{
    private readonly Dictionary<Vector2i, bool> _map = new();

    private List<Edge> edges = default!;

    public ColliderMap(Image<Rgba32> image)
    {
        Width = image.Width;
        Height = image.Height;

        var span = image.GetPixelSpan();

        for (var y = 0; y < Height; y++)
        for (var x = 0; x < Width; x++)
        {
            var imgPixel = span[Width * y + x];
            _map.Add(new Vector2i(x, Height - y), imgPixel.A != 0);
        }
    }

    public int Width { get; }
    public int Height { get; }

    public IEnumerator<Vector2i> GetEnumerator()
    {
        foreach (var (pos, isEnabled) in _map)
            if (isEnabled)
                yield return pos;
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
        } while (edg.TryFirstOrDefault(pr => pr.Start == a.End, out a));


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
                new Vector2i(vector2I.X - 1, vector2I.Y) // Влево
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
            .GroupBy(e => new { e.Start, e.End })
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
    public Edge(Vector2i start, Vector2i end)
    {
        Start = start;
        End = end;
    }

    public Vector2i Start { get; }
    public Vector2i End { get; }
}
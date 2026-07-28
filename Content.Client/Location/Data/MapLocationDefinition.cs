using System.Numerics;
using Content.Client.Location.Components;
using Content.Client.Location.Systems;
using Robust.Shared.ContentPack;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client.Location.Data;

[DataDefinition]
public sealed partial class MapLocationDefinition: ILocationDefinition
{
    [DataField] public EntProtoId WallsId = "Wall";
    [DataField] public ResPath? LightPath;
    [DataField] public ResPath? Map;
    [DataField] public ResPath Path;
    
    public void LoadLocation(Entity<LocationComponent> mapEntity, IEntityManager entityManager, LocationSystem locationSystem, IResourceManager resourceManager)
    {
        Map ??= new ResPath(Path.ToString().Replace(".png", ".map.png"));
        LightPath ??= new ResPath(Path.ToString().Replace(".png", ".overlay.png"));

        using var stream = resourceManager.ContentFileRead(Map.Value.ToString());
        var texture = Image.Load<Rgba32>(stream);
        var map = new ColliderMap(texture);
        
        foreach (var pos in map) entityManager.SpawnEntity(WallsId, new EntityCoordinates(mapEntity, pos - new Vector2(-0.5f, 0.5f)));
        
        entityManager.AddComponent<LocationMapDrawComponent>(mapEntity).Path = Path;

        if (resourceManager.ContentFileExists(Map.Value.ToString()))
        {
            entityManager.AddComponent<LocationLightComponent>(mapEntity).LightPath = LightPath.Value;
        }
    }

    public void OnEnter(Entity<LocationComponent> mapEntity, IEntityManager entityManager, LocationSystem locationSystem,
        IResourceManager resourceManager)
    {
        
    }

    public void OnExit(Entity<LocationComponent> mapEntity, IEntityManager entityManager, LocationSystem locationSystem,
        IResourceManager resourceManager)
    {
        
    }
}
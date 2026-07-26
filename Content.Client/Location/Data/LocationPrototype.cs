using System.Numerics;
using Content.Client.Background;
using Content.Client.Dialog.Data;
using Content.Client.Location.Components;
using Content.Client.Location.Systems;
using Robust.Shared.Audio;
using Robust.Shared.ContentPack;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client.Location.Data;

[Prototype]
public sealed partial class LocationPrototype : IPrototype
{
    [IdDataField] public string ID { get; private set; } = default!;
    
    [DataField] public HashSet<SoundSpecifier> AmbientSounds = [];
    [DataField] public HashSet<EntityDefinition> Entities = [];
    
    [DataField(required:true)] public ILocationDefinition Location = default!;
}

[ImplicitDataDefinitionForInheritors]
public partial interface ILocationDefinition
{
    public void LoadLocation(Entity<LocationComponent> mapEntity, IEntityManager entityManager, LocationSystem locationSystem, IResourceManager resourceManager);
    public void OnEnter(Entity<LocationComponent> mapEntity, IEntityManager entityManager, LocationSystem locationSystem, IResourceManager resourceManager);
    public void OnExit(Entity<LocationComponent> mapEntity, IEntityManager entityManager, LocationSystem locationSystem, IResourceManager resourceManager);
}

[DataDefinition]
public sealed partial class LocationWithBackgroundDefinition : ILocationDefinition
{
    [DataField(required:true)] public ResPath Background;

    public void LoadLocation(Entity<LocationComponent> mapEntity, IEntityManager entityManager, LocationSystem locationSystem,
        IResourceManager resourceManager)
    {
        entityManager.System<BackgroundSystem>().LoadBackground(mapEntity, Background);
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
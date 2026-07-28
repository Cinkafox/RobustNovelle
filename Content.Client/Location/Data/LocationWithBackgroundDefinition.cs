using Content.Client.Background;
using Content.Client.Location.Components;
using Content.Client.Location.Systems;
using Robust.Shared.ContentPack;
using Robust.Shared.Utility;

namespace Content.Client.Location.Data;

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
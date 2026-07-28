using Content.Client.Location.Components;
using Content.Client.Location.Systems;
using Robust.Shared.ContentPack;

namespace Content.Client.Location.Data;

[ImplicitDataDefinitionForInheritors]
public partial interface ILocationDefinition
{
    public void LoadLocation(Entity<LocationComponent> mapEntity, IEntityManager entityManager, LocationSystem locationSystem, IResourceManager resourceManager);
    public void OnEnter(Entity<LocationComponent> mapEntity, IEntityManager entityManager, LocationSystem locationSystem, IResourceManager resourceManager);
    public void OnExit(Entity<LocationComponent> mapEntity, IEntityManager entityManager, LocationSystem locationSystem, IResourceManager resourceManager);
}
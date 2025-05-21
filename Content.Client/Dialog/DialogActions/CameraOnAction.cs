using Content.Client.Camera.Components;
using Content.Client.Camera.Systems;
using Content.Client.Dialog.Components;
using Content.Client.Dialog.Data;
using Content.Client.Location.Systems;
using Robust.Shared.Prototypes;

namespace Content.Client.Dialog.DialogActions;

public sealed partial class CameraOnAction : IDialogAction
{
    [DataField] public EntProtoId Follow;
    public void Act(IDependencyCollection collection, Entity<DialogContainerComponent> actorUid)
    {
        var entMgr = collection.Resolve<IEntityManager>();
        
        if(!entMgr.TryGetComponent<CameraComponent>(actorUid, out var camera)) 
            throw new Exception("Camera not found!");
        
        if(!entMgr.System<LocationSystem>().TryGetLocationEntity(Follow, out var camFol))
            throw new Exception($"Entity {Follow} not found!");
            
        collection.Resolve<IEntityManager>().System<CameraSystem>().FollowTo(new Entity<CameraComponent>(actorUid, camera), camFol);
    }
}
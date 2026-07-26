using Content.Client.Dialog.Components;
using Content.Client.Dialog.Data;
using Content.Client.Location.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Client.Dialog.DialogActions;

public sealed partial class RotateCharacterOnAction : IDialogAction
{
    [DataField] public EntProtoId Prototype;

    [DataField] public Angle Rotation = 0;

    public void Act(IDependencyCollection collection, Entity<DialogContainerComponent> actorUid)
    {
        var entMgr = collection.Resolve<IEntityManager>();
        
        if (!entMgr.System<LocationSystem>().TryGetLocationEntity(actorUid, Prototype, out var rotateSubject))
            throw new Exception($"Entity {Prototype} not found!");

        collection.Resolve<IEntityManager>().System<TransformSystem>().SetLocalRotation(rotateSubject, Rotation);
    }
}
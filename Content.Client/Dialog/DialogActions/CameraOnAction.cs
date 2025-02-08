using Content.Client.Camera.Systems;
using Content.Client.Dialog.Data;
using Robust.Shared.Prototypes;

namespace Content.Client.Dialog.DialogActions;

public sealed partial class CameraOnAction : IDialogAction
{
    [DataField] public EntProtoId Follow;
    public void Act(IDependencyCollection collection)
    {
        collection.Resolve<IEntityManager>().System<CameraSystem>().FollowTo(Follow);
    }
}
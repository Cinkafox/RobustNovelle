using Content.Client.Dialog.Components;
using Content.Client.Dialog.Data;
using Content.Client.Scene.Data;
using Content.Client.Scene.Systems;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Client.Dialog.DialogActions;

[UsedImplicitly]
public sealed partial class LoadSceneAction : IDialogAction
{
    [DataField] public ProtoId<ScenePrototype> Prototype = default!;
    
    public void Act(IDependencyCollection collection, Entity<DialogContainerComponent> actorUid)
    {
        collection.Resolve<IEntityManager>().System<SceneSystem>().LoadScene(actorUid,Prototype);
    }
}
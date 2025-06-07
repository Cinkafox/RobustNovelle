using Content.Client.Dialog.Components;
using Content.Client.Dialog.Data;
using Content.Client.Menu;
using Content.Client.Scene.Systems;
using Robust.Client;
using Robust.Client.GameStates;
using Robust.Client.State;

namespace Content.Client.Dialog.DialogActions;

public sealed partial class EndGameAction : IDialogAction
{
    public void Act(IDependencyCollection collection, Entity<DialogContainerComponent> actorUid)
    {
        collection.Resolve<IEntityManager>().System<SceneSystem>().ShutdownScene();
    }
}
using Content.Game.Dialog.Data;
using Content.Game.Dialog.Systems;
using Content.Game.Scene.Manager;
using JetBrains.Annotations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Game.Dialog.DialogActions;

[UsedImplicitly]
public sealed partial class DefaultDialogAction : IDialogAction
{
    public void Act()
    {
        IoCManager.Resolve<EntityManager>().System<DialogSystem>().ContinueDialog();
    }
}
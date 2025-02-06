using Content.Client.Dialog.Data;
using Content.Client.Dialog.Systems;
using Content.Client.Scene.Manager;
using JetBrains.Annotations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Dialog.DialogActions;

[UsedImplicitly]
public sealed partial class DefaultDialogAction : IDialogAction
{
    public void Act()
    {
        IoCManager.Resolve<EntityManager>().System<DialogSystem>().ContinueDialog();
    }
}
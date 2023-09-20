using Cinka.Game.Dialog.Data;
using Cinka.Game.Scene.Manager;
using JetBrains.Annotations;
using Robust.Shared.IoC;

namespace Cinka.Game.Dialog.DialogActions;

[UsedImplicitly]
public sealed partial class DefaultDialogAction : IDialogAction
{
    public void Act()
    {
        IoCManager.Resolve<ISceneManager>().ContinueDialog();
    }
}
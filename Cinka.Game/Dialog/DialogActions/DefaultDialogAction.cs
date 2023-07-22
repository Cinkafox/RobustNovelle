using System;
using Cinka.Game.Dialog.Data;
using Cinka.Game.Scene.Manager;
using JetBrains.Annotations;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Dialog.DialogActions;

[UsedImplicitly]
public sealed class DefaultDialogAction : IDialogAction
{
    public void Act()
    {
        IoCManager.Resolve<ISceneManager>().ContinueDialog();
    }
}
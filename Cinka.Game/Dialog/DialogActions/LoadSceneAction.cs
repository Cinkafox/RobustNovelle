using Cinka.Game.Dialog.Data;
using Cinka.Game.Scene.Manager;
using JetBrains.Annotations;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Dialog.DialogActions;

[UsedImplicitly]
public sealed class LoadSceneAction : IDialogAction
{
    [DataField("prototype", required: true)]
    public string ScenePrototype;

    public void Act()
    {
        IoCManager.Resolve<ISceneManager>().LoadScene(ScenePrototype);
    }
}
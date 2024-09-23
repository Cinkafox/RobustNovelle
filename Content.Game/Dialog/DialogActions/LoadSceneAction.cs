using Content.Game.Dialog.Data;
using Content.Game.Scene.Data;
using Content.Game.Scene.Manager;
using JetBrains.Annotations;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Toolshed.TypeParsers;

namespace Content.Game.Dialog.DialogActions;

[UsedImplicitly]
public sealed partial class LoadSceneAction : IDialogAction
{
    [DataField] public ProtoId<ScenePrototype> Prototype = default!;
    
    public void Act()
    {
        IoCManager.Resolve<ISceneManager>().LoadScene(Prototype);
    }
}
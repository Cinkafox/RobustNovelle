using Cinka.Game.Audio.Managers;
using Cinka.Game.Dialog.Data;
using JetBrains.Annotations;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Dialog.DialogActions;

[UsedImplicitly]
public sealed class AudioAction : IDialogAction
{
    [DataField("prototype",required:true)] private string _audioPrototype = default!;

    public void Act()
    {
        IoCManager.Resolve<IAudioManager>().Play(_audioPrototype);
    }
}
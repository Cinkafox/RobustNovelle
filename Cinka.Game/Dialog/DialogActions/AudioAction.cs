using Cinka.Game.Audio.Managers;
using Cinka.Game.Dialog.Data;
using JetBrains.Annotations;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Dialog.DialogActions;

[UsedImplicitly]
public sealed class AudioBackgroundAction : IDialogAction
{
    [DataField("path")] public string Path = string.Empty;

    public void Act()
    {
        IoCManager.Resolve<IAudioManager>().PlayBackground(Path);
    }
}

[UsedImplicitly]
public sealed class AudioCleanBackgroundAction : IDialogAction
{
    [DataField("path")] public string Path = string.Empty;

    public void Act()
    {
        var asy = IoCManager.Resolve<IAudioManager>();
        asy.StopBackground();
        asy.PlayBackground(Path);
    }
}

[UsedImplicitly]
public sealed class AudioAction : IDialogAction
{
    [DataField("path")] public string Path = string.Empty;

    public void Act()
    {
        IoCManager.Resolve<IAudioManager>().Play(Path);
    }
}
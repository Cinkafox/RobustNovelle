using Cinka.Game.Audio.Data;
using Cinka.Game.Dialog.Data;
using JetBrains.Annotations;
using Robust.Client.Audio;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Dialog.DialogActions;

[UsedImplicitly]
public sealed partial class AudioAction : IDialogAction
{
    [DataField("prototype",required:true)] private string _audioPrototype = default!;

    public static EntityUid? Background;

    public void Act()
    {
        if (!IoCManager.Resolve<IPrototypeManager>().TryIndex<AudioPrototype>(_audioPrototype, out var prototype))
        {
            Logger.Error("Oh fuck! Could not play audio shit!");
            return;
        }

        var entMan = IoCManager.Resolve<IEntityManager>();
        var audioSystem = entMan.System<AudioSystem>();

        if (prototype.IsBackground)
        {
            audioSystem.Stop(Background);
            Background = audioSystem.PlayGlobal(prototype.Audio, Filter.Local(), true, AudioParams.Default.WithLoop(true).WithVolume(-6)).Value.Entity;
        }
        else
        {
            audioSystem.PlayGlobal(prototype.Audio, Filter.Local(), true, AudioParams.Default);
        }
    }
}
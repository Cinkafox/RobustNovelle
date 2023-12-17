using System;
using Cinka.Game.Audio.Data;
using Cinka.Game.Dialog.Data;
using JetBrains.Annotations;
using Robust.Client.Audio;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
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
    
    [DataField("effect")] private string _effectPrototype = String.Empty;

    public static EntityUid? Background;

    public static (EntityUid, AudioAuxiliaryComponent) Aux;
    public static (EntityUid, AudioEffectComponent) Eff;

    public bool first = true;

    public void Act()
    {
        if (!IoCManager.Resolve<IPrototypeManager>().TryIndex<AudioPrototype>(_audioPrototype, out var prototype))
        {
            Logger.Error("Oh fuck! Could not play audio shit!");
            return;
        }

        var entMan = IoCManager.Resolve<IEntityManager>();
        var audioSystem = entMan.System<AudioSystem>();

        if (first)
        {
            Aux = audioSystem.CreateAuxiliary();
            Eff = audioSystem.CreateEffect();
        }

        if (prototype.IsBackground)
        {
            audioSystem.Stop(Background);
            Background = PlayAudio(audioSystem,prototype.Audio, AudioParams.Default.WithLoop(true).WithVolume(-6));
        }
        else
        {
            PlayAudio(audioSystem,prototype.Audio, AudioParams.Default);
        }

        first = false;
    }

    private EntityUid PlayAudio(AudioSystem audioSystem,SoundSpecifier specifier, AudioParams audioParams)
    {
        var (uid,comp) =  audioSystem
            .PlayGlobal(specifier, Filter.Local(), true, audioParams).Value;
        
        if (IoCManager.Resolve<IPrototypeManager>().TryIndex<AudioPresetPrototype>(_effectPrototype,out var prototype))
        {
            //TODO: изуить поподробнее эту шнягу да
            audioSystem.SetEffectPreset(Eff.Item1,Eff.Item2,prototype);
            audioSystem.SetEffect(Aux.Item1,Aux.Item2,Eff.Item1);
            audioSystem.SetAuxiliary(uid,comp,Aux.Item1);
        }
        
        return uid;
    }
}
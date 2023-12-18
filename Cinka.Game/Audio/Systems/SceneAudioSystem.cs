using System.Collections.Generic;
using Cinka.Game.Audio.Data;
using Cinka.Game.Character.Managers;
using Cinka.Game.UserInterface.Systems.Dialog;
using Robust.Client.Audio;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Cinka.Game.Audio.Systems;

public sealed class SceneAudioSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly AudioSystem _audioSystem = default!;
    
    public static EntityUid? Background;

    public static Dictionary<string, EntityUid> EffectBank = new();

    public void Play(string prototypeName,string effect = "")
    {
        if (!_prototypeManager.TryIndex<AudioPrototype>(prototypeName, out var prototype))
        {
            Logger.Error("Oh fuck! Could not play audio shit!");
            return;
        }
        
        if (prototype.IsBackground)
        {
            _audioSystem.Stop(Background);
            Background = PlayAudio(prototype.Audio, AudioParams.Default.WithVolume(-6).WithLoop(true),effect);
        } 
        else
        {
            PlayAudio(prototype.Audio, AudioParams.Default,effect);
        }
        
    }
    
    private EntityUid PlayAudio(SoundSpecifier specifier, AudioParams audioParams,string effect = "")
    {
        var (uid,comp) =  _audioSystem
            .PlayGlobal(specifier, Filter.Local(), false, audioParams).Value;
        
        SwitchEffect(uid,comp,effect);
        
        return uid;
    }
    
    public void SwitchEffect(EntityUid uid,AudioComponent comp, string effect)
    {
        if (IoCManager.Resolve<IPrototypeManager>().TryIndex<AudioPresetPrototype>(effect,out var prototype))
        {
            if (!EffectBank.TryGetValue(effect, out var auxUid))
            {
                (auxUid, var auxComp) = _audioSystem.CreateAuxiliary();
                var (effectUid, effectComp) = _audioSystem.CreateEffect();
                _audioSystem.SetEffectPreset(effectUid,effectComp,prototype);
                _audioSystem.SetEffect(auxUid,auxComp,effectUid);
                EffectBank.Add(effect, auxUid);
            }
            
            _audioSystem.SetAuxiliary(uid,comp,auxUid);
        }
    }
}
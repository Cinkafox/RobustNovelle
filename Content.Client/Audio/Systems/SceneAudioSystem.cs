using Content.Client.Audio.Data;
using Content.Client.Gameplay;
using Robust.Client.Audio;
using Robust.Client.State;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.Audio.Systems;

public sealed class SceneAudioSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly AudioSystem _audioSystem = default!;
    [Dependency] private readonly IStateManager _stateManager = default!;

    private static Dictionary<string, EntityUid> EffectBank = new();

    public override void Initialize()
    {
        _stateManager.OnStateChanged += OnStateChanged;
    }

    private void OnStateChanged(StateChangedEventArgs ev)
    {
        if(ev.OldState is not GameplayState) return;
        var query = EntityQueryEnumerator<AudioComponent>();
        while (query.MoveNext(out var uid, out _))
        {
            _audioSystem.Stop(uid);
        }
    }

    public EntityUid Play(ProtoId<AudioPrototype> prototypeName)
    {
        if (!_prototypeManager.TryIndex(prototypeName, out var prototype))
        {
            throw new Exception("Could not play audio!");
        }
        
        return PlayAudio(prototype.Audio, prototype.AudioParams, prototype.Effect);
    }
    
    private EntityUid PlayAudio(SoundSpecifier specifier, AudioParams audioParams, ProtoId<AudioPresetPrototype>? effect = null)
    {
        var audio = _audioSystem
            .PlayGlobal(specifier, Filter.Local(), false, audioParams);
        
        if (audio is null) 
            throw new Exception("Could not play audio!");
        
        var (uid,comp) = audio.Value;

        if (effect != null) 
            SwitchEffect(uid, comp, effect.Value);

        return uid;
    }
    
    public void SwitchEffect(EntityUid uid,AudioComponent comp, ProtoId<AudioPresetPrototype> effect)
    {
        if (!_prototypeManager.TryIndex(effect, out var prototype)) 
            return;
        
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
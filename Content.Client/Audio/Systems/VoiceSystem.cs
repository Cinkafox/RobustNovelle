using Content.Client.Audio.Components;
using Content.Client.Dialog;
using Robust.Client.Audio;
using Robust.Shared.Audio;
using Robust.Shared.Timing;

namespace Content.Client.Audio.Systems;

public sealed partial class VoiceSystem : EntitySystem
{
    [Dependency] private AudioSystem _audioSystem = default!;
    [Dependency] private IGameTiming _gameTiming = default!;
    
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<VoiceComponent, DialogAppendEvent>(OnDialogAppend);
    }

    private void OnDialogAppend(EntityUid uid, VoiceComponent component, DialogAppendEvent args)
    {
        if(!args.Dialog.SayLetters || 
           _gameTiming.CurTime - component.LastVoiceTime < component.MinVoiceTimeSpan) 
            return;
        
        _audioSystem.PlayEntity(component.Voice, args.DialogEntity, uid, AudioParams.Default);
        component.LastVoiceTime = _gameTiming.CurTime;
    }
}
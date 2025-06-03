using Content.Client.Audio.Components;
using Content.Client.Dialog;
using Robust.Client.Audio;
using Robust.Shared.Audio;

namespace Content.Client.Audio.Systems;

public sealed class VoiceSystem : EntitySystem
{
    [Dependency] private readonly AudioSystem _audioSystem = default!;
    
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<VoiceComponent,DialogAppendEvent>(OnDialogAppend);
    }

    private void OnDialogAppend(EntityUid uid, VoiceComponent component, DialogAppendEvent args)
    {
        if (args.Dialog is not { Delay: > 10, SayLetters: true }) return;
        if (args.Dialog.SkipCounter == args.Dialog.SkipSayCount)
        {
            _audioSystem.PlayEntity(component.Voice, args.DialogEntity, uid, AudioParams.Default);
            args.Dialog.SkipCounter = 0;
        }
        else
        {
            args.Dialog.SkipCounter += 1;
        }
    }
}
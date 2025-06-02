using Content.Client.Audio.Components;
using Content.Client.Dialog;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Audio.Systems;

public sealed class VoiceSystem : EntitySystem
{
    [Dependency] private readonly SceneAudioSystem _audioSystem = default!;
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
            _audioSystem.Play(component.Voice);
            args.Dialog.SkipCounter = 0;
        }
        else
        {
            args.Dialog.SkipCounter += 1;
        }
    }
}
using Cinka.Game.Audio.Components;
using Cinka.Game.Dialog;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Cinka.Game.Audio.Systems;

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
        if(args.Dialog.Delay > 30)
            _audioSystem.Play(component.Voice);
    }
}
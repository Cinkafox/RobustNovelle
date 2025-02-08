using Content.Client.Audio.Systems;
using Content.Client.Dialog.Data;
using Robust.Client.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Dialog.DialogActions.Dev;

public sealed partial class SwitchEffectBackAction : IDialogAction
{
    [DataField("prototype", required: true)]
    public string Prototype = default!;
    public void Act(IDependencyCollection collection)
    {
        var entMan = collection.Resolve<IEntityManager>();
        var audioSystem = entMan.System<SceneAudioSystem>();

        if (entMan.TryGetComponent<AudioComponent>(SceneAudioSystem.Background, out var component))
        {
            audioSystem.SwitchEffect(SceneAudioSystem.Background.Value,component,Prototype);
        }
    }
}
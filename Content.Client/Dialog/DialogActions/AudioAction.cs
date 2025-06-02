using Content.Client.Audio.Systems;
using Content.Client.Dialog.Components;
using Content.Client.Dialog.Data;
using JetBrains.Annotations;

namespace Content.Client.Dialog.DialogActions;

[UsedImplicitly]
public sealed partial class AudioAction : IDialogAction
{
    [DataField("prototype",required:true)] private string _audioPrototype = default!;
    
    public void Act(IDependencyCollection collection, Entity<DialogContainerComponent> actorUid)
    {
       collection.Resolve<EntityManager>().System<SceneAudioSystem>().Play(_audioPrototype);
    }
}
using Content.Client.Dialog.Components;
using Content.Client.Dialog.Data;
using JetBrains.Annotations;
using Robust.Client.Audio;
using Robust.Shared.Audio;

namespace Content.Client.Dialog.DialogActions;

[UsedImplicitly]
public sealed partial class AudioAction : IDialogAction
{
    [DataField(required:true)] public SoundSpecifier Sound = default!;
    
    public void Act(IDependencyCollection collection, Entity<DialogContainerComponent> actorUid)
    {
        collection.Resolve<EntityManager>().System<AudioSystem>().PlayGlobal(Sound, actorUid);
    }
}
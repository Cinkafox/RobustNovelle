using System;
using Content.Game.Audio.Systems;
using Content.Game.Dialog.Data;
using JetBrains.Annotations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Game.Dialog.DialogActions;

[UsedImplicitly]
public sealed partial class AudioAction : IDialogAction
{
    [DataField("prototype",required:true)] private string _audioPrototype = default!;
    
    [DataField("effect")] private string _effectPrototype = String.Empty;
    
    public void Act()
    {
      IoCManager.Resolve<EntityManager>().System<SceneAudioSystem>().Play(_audioPrototype,_effectPrototype);
    }
}
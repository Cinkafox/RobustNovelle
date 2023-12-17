using System.Diagnostics.CodeAnalysis;
using Cinka.Game.Character.Components;
using Cinka.Game.UserInterface.Systems.Dialog;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Cinka.Game.Character.Managers;

public sealed class EmoteSystem : EntitySystem
{
    [Dependency] private readonly CharacterSystem _characterSystem = default!;
    [Dependency] private readonly IResourceCache _cache = default!;
    
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<DialogMessageStarted>(OnDialogMessageStarted);
    }

    private void OnDialogMessageStarted(DialogMessageStarted ev)
    {
        if (ev.CurrentDialog.Character != null 
            && _characterSystem.TryGetCharacter(ev.CurrentDialog.Character, out var characterComponent, out var uid) 
            && TryGetEmotesSprite(uid, out var resource) && resource.RSI.TryGetState(ev.CurrentDialog.Emote, out var state))
        {
            ev.DialogGui.SetEmote(state.Frame0);
        }
        else if (ev.CurrentDialog.IsDialog)
        {
            ev.DialogGui.SetEmote(null);
        }
    }

    public bool TryGetEmotesSprite(EntityUid uid, [NotNullWhen(true)] out RSIResource? resource,
        EmoteComponent? component = null)
    {
        resource = null;
        if(!Resolve(uid,ref component)) return false;

        return _cache.TryGetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / component.RsiPath,
            out resource);
    }
}
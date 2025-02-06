using System.Diagnostics.CodeAnalysis;
using Content.Client.Character.Components;
using Content.Client.Dialog;
using Content.Client.Dialog.Systems;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Client.Character.Systems;

public sealed class EmoteSystem : EntitySystem
{
    [Dependency] private readonly IResourceCache _cache = default!;
    [Dependency] private readonly DialogSystem _dialog = default!;
    
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<EmoteComponent,DialogStartedEvent>(OnEmoteStarted);
        SubscribeLocalEvent<DialogStartedEvent>(OnDialogStarted);
    }

    private void OnDialogStarted(DialogStartedEvent ev)
    {
        if(ev.Dialog is { IsDialog: true, Character: null })
            _dialog.SetEmote(null);
    }
    
    private void OnEmoteStarted(EntityUid uid, EmoteComponent component, DialogStartedEvent args)
    {
        if (TryGetEmotesSprite(uid, out var resource) && resource.RSI.TryGetState(args.Dialog.Emote, out var state))
            _dialog.SetEmote(state.Frame0);
        else if (args.Dialog.IsDialog)
            _dialog.SetEmote(null);
        
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
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Client.Character.Components;
using Content.Client.Location.Systems;
using Content.Client.Dialog.Data;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Client.Character.Systems;

public sealed class CharacterSystem : EntitySystem
{
    [Dependency] private readonly LocationSystem _locationManager = default!;
    [Dependency] private readonly IResourceCache _cache = default!;
    [Dependency] private readonly IOverlayManager _overlay = default!;
    
    public const int CharacterRenderingZIndex = 0;

    public override void Initialize()
    {
        _overlay.AddOverlay(new CharacterRenderingOverlay());
        SubscribeLocalEvent<CharacterComponent,ComponentInit>(OnComponentInit);
    }

    private void OnComponentInit(EntityUid uid, CharacterComponent component, ComponentInit args)
    {
        if(!_cache.TryGetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / component.RsiPath,
               out var rs)) return;

        component.Sprite = rs.RSI;
    }
    
    public bool TryGetCharacter(EntityUid locationEntity, EntProtoId? prototype,[NotNullWhen(true)] out CharacterComponent? component,out EntityUid uid)
    {
        uid = EntityUid.Invalid;
        component = null;
        
        if (prototype != null && !_locationManager.TryGetLocationEntity(locationEntity,prototype, out uid)) return false;
        
        return TryComp(uid, out component);
    }

    public void SetCharacterState(EntityUid locationEntity, string prototype, string state)
    {
        if (TryGetCharacter(locationEntity, prototype, out var data, out _))
            data.State = state;
    }
    
    public IEnumerable<CharacterComponent> EnumerateCharacters(EntityUid locationEntity)
    {
        foreach (var uid in _locationManager.GetLocationEnumerator(locationEntity))
        {
            if (TryComp<CharacterComponent>(uid, out var characterComponent) && characterComponent.Visible)
                yield return characterComponent;
        }
    }
}
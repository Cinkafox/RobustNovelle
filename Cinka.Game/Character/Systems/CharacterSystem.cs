using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Cinka.Game.Character.Components;
using Cinka.Game.Location.Managers;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Cinka.Game.Character.Systems;

public sealed class CharacterSystem : EntitySystem
{
    [Dependency] private readonly ILocationManager _locationManager = default!;
    [Dependency] private readonly IResourceCache _cache = default!;
    [Dependency] private readonly IOverlayManager _overlay = default!;
    
    private Dictionary<string, EntityUid> _characters = new();
    
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

    public void AddCharacter(Scene.Data.Character character)
    {
        var uid = Spawn(character.Entity,
            new MapCoordinates(Vector2.Zero, _locationManager.GetCurrentLocationId()));

        if (!TryComp<CharacterComponent>(uid, out var component))
        {
            QueueDel(uid);
            return;
        }

        component.Visible = character.Visible;
        
        _characters.Add(character.Entity,uid);
        
        SetCharacterState(character.Entity, component.State);
    }

    public void RemoveCharacter(string prototype)
    {
        _characters.Remove(prototype, out var uid);
        QueueDel(uid);
    }
    
    public bool TryGetCharacter(string? prototype,[NotNullWhen(true)] out CharacterComponent? component,out EntityUid uid)
    {
        uid = EntityUid.Invalid;
        component = null;
        
        if (prototype != null && !_characters.TryGetValue(prototype, out uid)) return false;
        
        return TryComp(uid, out component);
    }

    public void SetCharacterState(string prototype, string state)
    {
        if (TryGetCharacter(prototype, out var data, out _))
            data.State = state;
    }
    
    public IEnumerable<CharacterComponent> EnumerateCharacters()
    {
        foreach (var (_, uid) in _characters)
        {
            if (TryComp<CharacterComponent>(uid, out var characterComponent) && characterComponent.Visible)
                yield return characterComponent;
        }
    }
    
    public void ClearCharacters()
    {
        foreach (var (proto,_) in _characters)
        {
           RemoveCharacter(proto);
        }
    }
}
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Cinka.Game.Camera.Manager;
using Cinka.Game.Character.Components;
using Cinka.Game.Location.Managers;
using Robust.Client.GameObjects;
using Robust.Client.Physics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Cinka.Game.Character.Managers;

public sealed class CharacterSystem : EntitySystem
{
    [Dependency] private readonly ILocationManager _locationManager = default!;
    [Dependency] private readonly IResourceCache _cache = default!;
    [Dependency] private readonly ICameraManager _cameraManager = default!;
    [Dependency] private readonly PhysicsSystem _physicsSystem = default!;
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly IMapManager _map = default!;
    
    private readonly Dictionary<string, EntityUid> _characters = new();

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<CharacterComponent,ComponentInit>(OnComponentInit);
    }

    private void OnComponentInit(EntityUid uid, CharacterComponent component, ComponentInit args)
    {
        if(!_cache.TryGetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / component.RsiPath,
               out var rs)) return;

        component.Sprite = rs.RSI;
    }

    public void AddCharacter(Data.Character character)
    {
        if(!_characters.TryGetValue(character.Entity,out var uid))
        {
            uid = EntityManager.Spawn(character.Entity,
                new MapCoordinates(0, 0, _locationManager.GetCurrentLocationId()),character.Components);
            _characters.Add(character.Entity,uid);
        }

        if (!TryComp<CharacterComponent>(uid, out var component))
        {
            Logger.Error($"Entity ${character.Entity} does not have CharacterComponent!");
            RemoveCharacter(character.Entity);
            return;
        }
        
        SetCharacterState(character.Entity, component.State);

        if (character.IsPlayer)
        {
            _cameraManager.AttachEntity(uid);
        }
        
        if (character.Position.HasValue)
        {
            _transform.SetLocalPosition(uid,character.Position.Value);
        }
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
    
    public string GetCharacterState(string prototype)
    {
        if (TryGetCharacter(prototype, out var data, out _))
            return data.State;
        return "default";
    }

    public void SetCharacterState(string prototype, string state)
    {
        if (TryGetCharacter(prototype, out var data, out _))
            data.State = state;
    }
    
    public IEnumerable<CharacterComponent> EnumerateVisibleCharacters()
    {
        foreach (var (_, uid) in _characters)
        {
            if (TryComp<CharacterComponent>(uid, out var characterComponent) && characterComponent.IsVisible)
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
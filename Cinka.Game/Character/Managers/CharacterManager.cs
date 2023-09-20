using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cinka.Game.Character.Components;
using Cinka.Game.Location.Managers;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Cinka.Game.Character.Managers;

public sealed class CharacterManager : ICharacterManager
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly ILocationManager _locationManager = default!;
    private readonly Dictionary<string, CharacterData> _characters = new();

    public void Initialize()
    {
        IoCManager.InjectDependencies(this);
    }

    public void AddCharacter(string prototype)
    {
        var uid = _entityManager.SpawnEntity(prototype,
            new MapCoordinates(Vector2.Zero, _locationManager.GetCurrentLocationId()));

        if (!_entityManager.TryGetComponent<CharacterComponent>(uid, out var component))
        {
            _entityManager.QueueDeleteEntity(uid);
            return;
        }

        var data = ExtractCharacterData(uid, component);

        if (data == null)
        {
            _entityManager.QueueDeleteEntity(uid);
            return;
        }

        _characters.Add(prototype, data);
        SetCharacterState(prototype, component.State);
    }

    public void ClearCharacters()
    {
        foreach (var characters in _characters.Keys.ToList()) RemoveCharacter(characters);
    }

    public int Count()
    {
        return _characters.Count;
    }

    public void RemoveCharacter(string prototype)
    {
        if (!_characters.TryGetValue(prototype, out var data))
            return;

        _characters.Remove(prototype);
        _entityManager.QueueDeleteEntity(data.Uid);
    }

    public string GetCharacterState(string prototype)
    {
        if (TryGetCharacter(prototype, out var data))
            return data.State;
        return "default";
    }

    public void SetCharacterState(string prototype, string state)
    {
        if (TryGetCharacter(prototype, out var data))
            data.State = state;
    }

    public bool TryGetCharacter(string prototype,[NotNullWhen(true)] out CharacterData? data)
    {
        return _characters.TryGetValue(prototype, out data);
    }

    public IEnumerable<CharacterData> EnumerateCharacters()
    {
        foreach (var (_, data) in _characters) yield return data;
    }


    private CharacterData? ExtractCharacterData(EntityUid uid, CharacterComponent component)
    {
        if (!TryGetRSI(component.RsiPath, out var rsi))
            return null;

        var data = new CharacterData(rsi)
        {
            Uid = uid
        };

        return data;
    }

    private bool TryGetRSI(string rsiPath,[NotNullWhen(true)] out RSI? rsi)
    {
        rsi = null;
        if (!StaticIoC.ResC.TryGetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / rsiPath,
                out var rs)) return false;

        rsi = rs.RSI;
        return true;
    }
}
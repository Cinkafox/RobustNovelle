using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cinka.Game.Character.Managers;

public interface ICharacterManager
{
    public void Initialize();
    public void AddCharacter(Scene.Data.Character character);
    public void ClearCharacters();
    public int Count();
    public void RemoveCharacter(string prototype);
    public bool TryGetCharacter(string prototype,[NotNullWhen(true)] out CharacterData? data);
    public string GetCharacterState(string prototype);
    public void SetCharacterState(string prototype, string state);
    public IEnumerable<CharacterData> EnumerateCharacters();
}
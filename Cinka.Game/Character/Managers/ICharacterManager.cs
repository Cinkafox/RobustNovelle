using System.Collections.Generic;

namespace Cinka.Game.Character.Managers;

public interface ICharacterManager
{
    public void Initialize();
    public void AddCharacter(string prototype);
    public void ClearCharacters();
    public void RemoveCharacter(string prototype);
    public bool TryGetCharacter(string prototype, out CharacterData? data);
    public string GetCharacterState(string prototype);
    public void SetCharacterState(string prototype, string state);
    public IEnumerable<CharacterData> EnumerateCharacters();
}
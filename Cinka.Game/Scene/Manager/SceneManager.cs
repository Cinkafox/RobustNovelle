using System;
using Cinka.Game.Character.Systems;
using Cinka.Game.Dialog.Data;
using Cinka.Game.Dialog.Systems;
using Cinka.Game.Location.Managers;
using Cinka.Game.Scene.Data;
using Cinka.Game.UserInterface.Systems.Dialog;
using Microsoft.CodeAnalysis;
using Robust.Client;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

namespace Cinka.Game.Scene.Manager;

public sealed class SceneManager : ISceneManager
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IGameController _gameController = default!;
    [Dependency] private readonly ILocationManager _locationManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
    [Dependency] private readonly ISerializationManager _serializationManager = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;

    private CharacterSystem _characterSystem = default!;
    private DialogSystem _dialogSystem = default!;
    private ScenePrototype? _currentScene;

    public void Initialize()
    {
        IoCManager.InjectDependencies(this);
        
        _characterSystem = _entityManager.System<CharacterSystem>();
        _dialogSystem = _entityManager.System<DialogSystem>();
        
        LoadScene(_cfg.GetCVar(CCVars.CCVars.LastScenePrototype));
    }

    public void LoadScene(string prototype)
    {
        CleanupScene();

        if (!_prototypeManager.TryIndex<ScenePrototype>(prototype, out var proto))
        {
            _cfg.SetCVar("game.last_scene", "default");
            throw new Exception($"Scene {prototype} not found!");
        }
        
        _currentScene = _serializationManager.CreateCopy(proto, notNullableOverride:true);

        _locationManager.LoadLocation(_currentScene.Location);
        foreach (var characterPrototype in _currentScene.Characters) _characterSystem.AddCharacter(characterPrototype);
        foreach (var dialog in _currentScene.Dialogs) _dialogSystem.AddDialog(dialog);
        
        _dialogSystem.ContinueDialog();
    }
    

    public ScenePrototype? GetCurrentScene()
    {
        return _currentScene;
    }

    public void SaveScenePosition()
    {
        if (_currentScene != null) _cfg.SetCVar(CCVars.CCVars.LastScenePrototype, _currentScene.ID);
    }

    public void CleanupScene()
    {
        _characterSystem.ClearCharacters();
        _dialogSystem.CleanupDialog();
        _currentScene = null;
    }
}
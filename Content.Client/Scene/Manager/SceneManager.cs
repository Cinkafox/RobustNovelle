using Content.Client.Character.Systems;
using Content.Client.Dialog.Systems;
using Content.Client.Scene.Data;
using Content.Client.Camera.Systems;
using Content.Client.Location.Systems;
using Robust.Client.Graphics;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.Scene.Manager;

public sealed class SceneManager : ISceneManager
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly ISerializationManager _serializationManager = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IClyde _clyde = default!;

    private CharacterSystem _characterSystem = default!;
    private DialogSystem _dialogSystem = default!;
    private ScenePrototype? _currentScene;

    public void Initialize()
    {
        IoCManager.InjectDependencies(this);
        
        _characterSystem = _entityManager.System<CharacterSystem>();
        _dialogSystem = _entityManager.System<DialogSystem>();
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
        _dialogSystem.CleanupDialog();
        _characterSystem.HideAll();
        _currentScene = null;
    }
}
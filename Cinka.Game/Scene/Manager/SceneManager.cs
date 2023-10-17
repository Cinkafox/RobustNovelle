using System;
using Cinka.Game.Character.Managers;
using Cinka.Game.Dialog.Data;
using Cinka.Game.Location.Managers;
using Cinka.Game.Scene.Data;
using Cinka.Game.UserInterface.Systems.Dialog;
using Robust.Client;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

namespace Cinka.Game.Scene.Manager;

public sealed class SceneManager : ISceneManager
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly ICharacterManager _characterManager = default!;
    [Dependency] private readonly IGameController _gameController = default!;
    [Dependency] private readonly ILocationManager _locationManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
    [Dependency] private readonly ISerializationManager _serializationManager = default!;

    private ScenePrototype? _currentScene;

    private DialogUIController _dialogUiController = default!;

    public void Initialize()
    {
        IoCManager.InjectDependencies(this);
        _dialogUiController = _userInterfaceManager.GetUIController<DialogUIController>();
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
        foreach (var characterPrototype in _currentScene.Characters) _characterManager.AddCharacter(characterPrototype);
        
        ContinueDialog();
    }

    public void ContinueDialog()
    {
        if (_currentScene == null || _dialogUiController.IsMessage) return;
        if (_currentScene.Dialogs.Count == 0)
        {
            _gameController.Shutdown("Конец");
            return;
        }

        var currentDialog = _currentScene.Dialogs[0];
        if (currentDialog.NewDialog) _dialogUiController.ClearAllDialog();
        if (IsEmptyString(currentDialog.Text))
        {
            currentDialog.IsDialog = false;
            if (currentDialog.Buttons.Count == 0) currentDialog.SkipDialog = true;
            else if (currentDialog.Actions.Count != 0)
                throw new Exception($"Долбоеб блять какие действие с текстом? {currentDialog.Text}");
        }
        
        _dialogUiController.AppendText(currentDialog);
        _currentScene.Dialogs.RemoveAt(0);
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
        _characterManager.ClearCharacters();
        _dialogUiController.ClearAllDialog();
        _currentScene = null;
    }

    private bool IsEmptyString(string text)
    {
        return string.IsNullOrEmpty(text) || text == " ";
    }
}
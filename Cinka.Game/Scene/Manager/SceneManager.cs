using System;
using System.Linq;
using Cinka.Game.Character.Managers;
using Cinka.Game.Dialog.DialogActions;
using Cinka.Game.Location.Managers;
using Cinka.Game.Scene.Data;
using Cinka.Game.UserInterface.Systems.Dialog;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

namespace Cinka.Game.Scene.Manager;

public sealed class SceneManager : ISceneManager
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly ILocationManager _locationManager = default!;
    [Dependency] private readonly ICharacterManager _characterManager = default!;
    [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;

    public SceneData? CurrentScene;
    
    private DialogUIController _dialogUiController;
    
    public void Initialize()
    {
        IoCManager.InjectDependencies(this);
        _dialogUiController = _userInterfaceManager.GetUIController<DialogUIController>();
        LoadScene("default");
    }

    public void LoadScene(string prototype)
    {
        CleanupScene();
        
        if (!_prototypeManager.TryIndex<ScenePrototype>(prototype, out var proto))
            throw new Exception($"Scene {prototype} not found!");
        
        _locationManager.LoadLocation(proto.LocationPrototype);
        foreach (var characterPrototype in proto.Characters)
        {
            _characterManager.AddCharacter(characterPrototype);
        }
        
        CurrentScene = CopyToData(proto);
        ContinueDialog();
    }

    public void CleanupScene()
    {
        _characterManager.ClearCharacters();
        _dialogUiController.ClearAllDialog();
        CurrentScene = null;
    }

    public void ContinueDialog()
    {
        if(CurrentScene == null || CurrentScene.Dialogs.Count == 0 || _dialogUiController.IsMessage) return;

        var currentDialog = CurrentScene.Dialogs[0];

        if(currentDialog.NewDialog) _dialogUiController.ClearAllDialog();
        _dialogUiController.AppendText(currentDialog);
        
        CurrentScene.Dialogs.RemoveAt(0);
    }
    
    public SceneData? GetCurrentScenePrototype()
    {
        return CurrentScene;
    }

    private SceneData CopyToData(ScenePrototype prototype)
    {
        return new SceneData()
        {
            Characters = prototype.Characters,
            Dialogs = prototype.Dialogs.ToList(),
            LocationPrototype = prototype.LocationPrototype
        };
    }
}
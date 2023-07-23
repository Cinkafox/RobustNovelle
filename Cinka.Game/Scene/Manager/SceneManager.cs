using System;
using System.Collections.Generic;
using System.Linq;
using Cinka.Game.Character.Managers;
using Cinka.Game.Location.Managers;
using Cinka.Game.Scene.Data;
using Cinka.Game.UserInterface.Systems.Dialog;
using Robust.Client;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Cinka.Game.Scene.Manager;

public sealed class SceneManager : ISceneManager
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly ILocationManager _locationManager = default!;
    [Dependency] private readonly ICharacterManager _characterManager = default!;
    [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
    [Dependency] private readonly IGameController _gameController = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;

    public SceneData? CurrentScene;
    
    private DialogUIController _dialogUiController;
    
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
            _cfg.SetCVar("game.last_scene","default");
            throw new Exception($"Scene {prototype} not found!");
        }
        
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
        if(CurrentScene == null  || _dialogUiController.IsMessage) return;
        if (CurrentScene.Dialogs.Count == 0)
        {
            _gameController.Shutdown("Конец");
            return;
        }
        
        var currentDialog = CurrentScene.Dialogs[0];

        if(currentDialog.NewDialog) _dialogUiController.ClearAllDialog();
        _dialogUiController.AppendText(currentDialog);
        
        CurrentScene.Dialogs.RemoveAt(0);
    }
    
    public SceneData? GetCurrentScene()
    {
        return CurrentScene;
    }

    public void SaveScenePosition()
    {
        if (CurrentScene != null)
        {
            _cfg.SetCVar(CCVars.CCVars.LastScenePrototype,CurrentScene.ID);
        }
    }

    private SceneData CopyToData(ScenePrototype prototype)
    {
        return new SceneData()
        {
            ID = prototype.ID,
            Characters = prototype.Characters,
            Dialogs = CopyDialogs(prototype.Dialogs),
            LocationPrototype = prototype.LocationPrototype
        };
    }

    private List<Dialog.Data.Dialog> CopyDialogs(List<Dialog.Data.Dialog> dialogs)
    {
        var copiedDialog = new List<Dialog.Data.Dialog>();
        foreach (var dialog in dialogs)
        {
            copiedDialog.Add(new Dialog.Data.Dialog()
            {
                Actions = dialog.Actions.ToList(),
                Delay = dialog.Delay,
                Name = dialog.Name + "",
                NewDialog = dialog.NewDialog,
                SkipCommand = dialog.SkipCommand,
                Text = dialog.Text + ""
            });
        }

        return copiedDialog;
    }
}
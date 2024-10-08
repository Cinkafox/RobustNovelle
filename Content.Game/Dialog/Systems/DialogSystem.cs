using System;
using System.Collections.Generic;
using System.Linq;
using Content.Game.Character.Systems;
using Content.Game.Dialog.Data;
using Content.Game.Dialog.DialogActions;
using Content.Game.Input;
using Content.Game.Location.Managers;
using Content.Game.Menu;
using Content.Game.UserInterface.Systems.Dialog;
using Robust.Client;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;

namespace Content.Game.Dialog.Systems;

public sealed class DialogSystem : EntitySystem
{
    [Dependency] private readonly IGameController _gameController = default!;
    [Dependency] private readonly IStateManager _stateManager = default!;
    [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
    [Dependency] private readonly CharacterSystem _characterSystem = default!;
    [Dependency] private readonly IInputManager _input = default!;
    
    private DialogUIController _dialogUiController = default!;
    
    private List<Game.Dialog.Data.Dialog> _dialogQueue = [];
    
    private string? _textQueue = null;

    public bool HasDialog => _dialogQueue.Count > 0;
    
    public Dialog.Data.Dialog CurrentDialog => _dialogQueue[0];

    public override void Initialize()
    {
        base.Initialize();
        _dialogUiController = _userInterfaceManager.GetUIController<DialogUIController>();
        var cmdhandler = InputCmdHandler.FromDelegate(_ =>
            SkipMessage());
        
        _input.SetInputCommand(ContentKeyFunctions.SkipDialog,cmdhandler);
        _input.SetInputCommand(EngineKeyFunctions.UIClick,cmdhandler);
        
        SubscribeLocalEvent<DialogEndedEvent>(OnDialogEnd);
    }

    private void OnDialogEnd(DialogEndedEvent ev)
    {
        _dialogQueue.RemoveAt(0);
        
        foreach (var action in ev.Dialog.Actions.ToList())
        {
            action.Act();
        }
        
        foreach (var choise in ev.Dialog.Choices.ToList())
        {
            _dialogUiController.AddButton(choise);
        }
    }

    public void AddDialog(Dialog.Data.Dialog dialog)
    {
        Logger.Debug($"Added {dialog.Text}");
        _dialogQueue.Add(dialog);
    }

    public void SpeedupDialog()
    {
        if(!HasDialog || CurrentDialog.DontLetSkip) return;
        CurrentDialog.Delay = 2;
    }
    
    public void SkipMessage()
    {
        if(_textQueue != null) SpeedupDialog();
        else
        {
            var btns = _dialogUiController.GetDialogButtons();
            if(btns.Count == 1) btns[0].DialogAction.Act();
        }
    }

    private void SetDialogText(string text)
    {
        _textQueue = text;
    }

    private char NextDialogLetter()
    {
        if (_textQueue == null) return ' ';
        var a = _textQueue[0];
        _textQueue = _textQueue.Substring(1);

        return a;
    }

    public void CleanupDialog()
    {
        _dialogUiController.ClearDialogs();
        _dialogQueue.Clear();
        _textQueue = null;
    }

    public void SetEmote(Texture? texture)
    {
        _dialogUiController.SetEmote(texture);
    }
    
    public void ContinueDialog()
    {
        if (_dialogQueue.Count == 0)
        {
            _stateManager.RequestStateChange<MenuState>();
            return;
        }
        
        SetDialogText(CurrentDialog.Text);
        
        if (CurrentDialog.NewDialog) _dialogUiController.ClearDialogs();
        
        if (IsEmptyString(CurrentDialog.Text))
        {
            CurrentDialog.IsDialog = false;
            if (CurrentDialog.Choices.Count == 0)
                CurrentDialog.SkipDialog = true;
            
            else if (CurrentDialog.Actions.Count != 0)
                throw new Exception($"Долбоеб блять какие действие с текстом? {CurrentDialog.Text}");
        }
        
        if (CurrentDialog.Choices.Count == 0)
        {
            if (CurrentDialog.SkipDialog)
            {
                CurrentDialog.Actions.Add(new DefaultDialogAction());
            }
            else
                CurrentDialog.Choices.Add(new DialogButton() { Name = Loc.GetString("dialog-continue") });
        }

        _characterSystem.TryGetCharacter(CurrentDialog.Character, out _, out var characterUid);
        
        if (CurrentDialog.Name == null && CurrentDialog.Character != null && TryComp<MetaDataComponent>(characterUid, out var metadata))
        {
            CurrentDialog.Name = metadata.EntityName;
        }

        if (CurrentDialog.Name != null && _dialogUiController.IsEmpty())
        {
            _dialogUiController.AppendLabel($"[bold]{CurrentDialog.Name}[/bold]: ");
        }

        var startedEv = new DialogStartedEvent(CurrentDialog);

        if (characterUid.IsValid())
        {
            RaiseLocalEvent(characterUid, startedEv);
        }
        else
        {
            RaiseLocalEvent(startedEv);
        }
    }
    

    public override void FrameUpdate(float frameTime)
    {
        base.FrameUpdate(frameTime);
        
        if(_dialogQueue.Count == 0 || _textQueue == null) return;
        
        if (string.IsNullOrEmpty(_textQueue))
        {
            _textQueue = null;
            RaiseLocalEvent(new DialogEndedEvent(CurrentDialog));
            return;
        }

        if (CurrentDialog.PassedTime < CurrentDialog.Delay)
        {
            CurrentDialog.PassedTime += frameTime * 1000;
            return;
        }
        
        CurrentDialog.PassedTime = 0;

        if (_characterSystem.TryGetCharacter(CurrentDialog.Character, out _, out var characterUid))
        {
            RaiseLocalEvent(characterUid,new DialogAppendEvent(CurrentDialog));
        }
        
        _dialogUiController.AppendLetter(NextDialogLetter());
    }
    
    private bool IsEmptyString(string text)
    {
        return string.IsNullOrEmpty(text) || text == " ";
    }

}
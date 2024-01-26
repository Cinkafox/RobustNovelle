using System;
using System.Collections.Generic;
using System.Linq;
using Cinka.Game.Character.Components;
using Cinka.Game.Character.Managers;
using Cinka.Game.Dialog.Components;
using Cinka.Game.Dialog.Data;
using Cinka.Game.Dialog.DialogActions;
using Cinka.Game.Input;
using Cinka.Game.Location.Managers;
using Cinka.Game.UserInterface.Systems.Dialog;
using Robust.Client;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

namespace Cinka.Game.Dialog.Systems;

public sealed partial class DialogSystem : EntitySystem
{
    [Dependency] private readonly IGameController _gameController = default!;
    [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
    [Dependency] private readonly CharacterSystem _characterSystem = default!;
    [Dependency] private readonly IInputManager _input = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    
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

        CommandBinds.Builder.Bind(ContentKeyFunctions.SkipDialog, cmdhandler).Register<DialogSystem>();
        
        SubscribeLocalEvent<DialogEndedEvent>(OnDialogEnd);
        InitializeDialog();
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
    
    public void LoadDialog(string proto)
    {
        if(!_prototypeManager.TryIndex<DialogPrototype>(proto,out var dialogPrototype)) return;

        foreach (var characterPrototype in dialogPrototype.Characters)
        {
            if(!_characterSystem.TryGetCharacter(characterPrototype,out var characterComponent,out var uid)) continue;
            characterComponent.IsVisible = true;
            EnsureComp<OnDialogComponent>(uid);
        }
        foreach (var dialog in dialogPrototype.Dialogs) AddDialog(dialog);
        
        ContinueDialog();
    }

    public void AddDialog(Dialog.Data.Dialog dialog)
    {
        Logger.Debug($"Added {dialog.Text}");
        _dialogQueue.Add(dialog);
    }

    public void SpeedupDialog()
    {
        if(!HasDialog || CurrentDialog.DontLetSkip) return;
        CurrentDialog.Delay = 10;
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
            Hide();
            return;
        }
        
        _dialogUiController.Show();
        
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

    private void Hide()
    {
        var query = EntityQueryEnumerator<OnDialogComponent,CharacterComponent>();
        while (query.MoveNext(out var uid, out var dialogComponent,out var characterComponent))
        {
            RemComp(uid, dialogComponent);
            characterComponent.IsVisible = false;
        }
            
        _dialogUiController.Hide();
    }

    public bool IsVisible()
    {
        return _dialogUiController.IsVisible();
    }
    

    public override void FrameUpdate(float frameTime)
    {
        base.FrameUpdate(frameTime);
        
        if(_dialogQueue.Count == 0 || _textQueue == null) return;
        
        if (IsEmptyString(_textQueue))
        {
            _textQueue = null;
            RaiseLocalEvent(new DialogEndedEvent(CurrentDialog));
            return;
        }
        
        CurrentDialog.PassedTime += frameTime * 1000;

        if (!(CurrentDialog.PassedTime >= CurrentDialog.Delay)) return;
        
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
using System.Linq;
using Content.Client.Camera.Systems;
using Content.Client.Character.Systems;
using Content.Client.Input;
using Content.Client.Interaction.Components;
using Content.Client.Location.Systems;
using Content.Client.Movement;
using Content.Client.UserInterface.Systems.Dialog;
using Content.Client.Character.Components;
using Content.Client.Dialog.Data;
using Content.Client.Dialog.DialogActions;
using Content.Client.Menu;
using Robust.Client;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Animations;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;

namespace Content.Client.Dialog.Systems;

public sealed partial class DialogSystem : EntitySystem
{
    [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
    [Dependency] private readonly CharacterSystem _characterSystem = default!;
    [Dependency] private readonly IInputManager _input = default!;
    [Dependency] private readonly IClyde _clyde = default!;
    [Dependency] private readonly LocationSystem _location = default!;
    [Dependency] private readonly CameraSystem _cameraSystem = default!;
    [Dependency] private readonly AnimationPlayerSystem _animationPlayerSystem = default!;
    
    private DialogUIController _dialogUiController = default!;
    
    private List<Data.Dialog> _dialogQueue = [];
    
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
            action.Act(IoCManager.Instance!);
        }
        
        foreach (var choise in ev.Dialog.Choices.ToList())
        {
            _dialogUiController.AddButton(choise);
        }
    }

    public void AddDialog(Dialog.Data.Dialog dialog)
    {
        if(_dialogQueue.Count == 0) Show();
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
            if(btns.Count == 1) btns[0].DialogAction.Act(IoCManager.Instance!);
        }
    }

    private EntityUid? GetControlledUid()
    {
        return _cameraSystem.CameraUid?.Owner;
    }
    
    private void Show()
    {
        _dialogUiController.Show();
        if (TryComp<InteractionComponent>(GetControlledUid(), out var interactionComponent))
        {
            interactionComponent.IsEnabled = false;
        }
        if (TryComp<InputMoverComponent>(GetControlledUid(), out var inputMoverComponent))
        {
            inputMoverComponent.IsEnabled = false;
        }
    }

    private void Hide()
    {
        _dialogUiController.Hide();
        if (TryComp<InteractionComponent>(GetControlledUid(), out var interactionComponent))
        {
            interactionComponent.IsEnabled = true;
        }
        if (TryComp<InputMoverComponent>(GetControlledUid(), out var inputMoverComponent))
        {
            inputMoverComponent.IsEnabled = true;
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

        LoadLocation();
        SetTitle();
        SetDialogText(CurrentDialog.Text);
        EnsureDialogs();
        EnsureChoices();
        ShowCharacters();
        HideCharacters();
        
        _characterSystem.TryGetCharacter(CurrentDialog.Character?.ToString(), out _, out var characterUid);
        
        if (CurrentDialog.Name == null && CurrentDialog.Character != null)
        {
            CurrentDialog.Name = MetaData(characterUid).EntityName;
        }

        if (CurrentDialog.Name != null && _dialogUiController.IsEmpty())
        {
            _dialogUiController.AppendLabel($"[bold]{CurrentDialog.Name}[/bold]: ");
        }

        var startedEv = new DialogStartedEvent(CurrentDialog);

        if (characterUid.IsValid())
            RaiseLocalEvent(characterUid, startedEv);
        else
            RaiseLocalEvent(startedEv);
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

        if (_characterSystem.TryGetCharacter(CurrentDialog.Character?.ToString(), out _, out var characterUid))
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
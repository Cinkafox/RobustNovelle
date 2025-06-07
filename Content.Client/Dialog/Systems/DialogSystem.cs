using System.Linq;
using Content.Client.Camera.Systems;
using Content.Client.Character.Systems;
using Content.Client.Input;
using Content.Client.Interaction.Components;
using Content.Client.Location.Systems;
using Content.Client.Movement;
using Content.Client.UserInterface.Systems.Dialog;
using Content.Client.Character.Components;
using Content.Client.Dialog.Components;
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
using Robust.Shared.Player;

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
    
    public override void Initialize()
    {
        base.Initialize();
        _dialogUiController = _userInterfaceManager.GetUIController<DialogUIController>();
        
        var cmdhandler = new SkipDialogHandler(this);
        CommandBinds.Builder
            .Bind(ContentKeyFunctions.SkipDialog, cmdhandler)
            .Bind(EngineKeyFunctions.UIClick, cmdhandler)
            .Register<DialogSystem>();
        
        SubscribeLocalEvent<DialogContainerComponent, DialogEndedEvent>(OnDialogEnd);
    }

    public Entity<DialogContainerComponent> EnsureDialogComp(ICommonSession? commonSession)
    {
        if(commonSession is null) 
            throw new NullReferenceException("CommonSession is null");
        if(!commonSession.AttachedEntity.HasValue) 
            throw new NullReferenceException("AttachedEntity is null");
        var entityUid = commonSession.AttachedEntity.Value;
        if(!TryComp<DialogContainerComponent>(entityUid, out var component)) 
            throw new InvalidOperationException("Did not find component of type DialogContainerComponent");
        return new Entity<DialogContainerComponent>(entityUid, component);
    }

    private void OnDialogEnd(Entity<DialogContainerComponent> ent,ref DialogEndedEvent ev)
    {
        ent.Comp.DialogQueue.RemoveAt(0);
        
        foreach (var action in ev.Dialog.Actions.ToList())
        {
            action.Act(IoCManager.Instance!, ent);
        }
        
        foreach (var choise in ev.Dialog.Choices.ToList())
        {
            _dialogUiController.AddButton(choise, ent);
        }
    }

    public void AddDialog(Entity<DialogContainerComponent> ent, Dialog.Data.Dialog dialog)
    {
        if(ent.Comp.DialogQueue.Count == 0) Show(ent);
        ent.Comp.DialogQueue.Add(dialog);
    }

    public void SpeedupDialog(Entity<DialogContainerComponent> ent)
    {
        if(!ent.Comp.HasDialog || ent.Comp.CurrentDialog.DontLetSkip) return;
        ent.Comp.CurrentDialog.Delay = 2;
    }
    
    public void SkipMessage(Entity<DialogContainerComponent> ent)
    {
        if(ent.Comp.TextQueue != null) SpeedupDialog(ent);
        else
        {
            var btns = _dialogUiController.GetDialogButtons();
            if(btns.Count == 1) btns[0].DialogAction.Act(IoCManager.Instance!, ent);
        }
    }
    
    private void Show(Entity<DialogContainerComponent> ent)
    {
        _dialogUiController.Show();
        if (TryComp<InteractionComponent>(ent, out var interactionComponent))
        {
            interactionComponent.IsEnabled = false;
        }
        if (TryComp<InputMoverComponent>(ent, out var inputMoverComponent))
        {
            inputMoverComponent.IsEnabled = false;
        }
    }

    private void Hide(Entity<DialogContainerComponent> ent)
    {
        _dialogUiController.Hide();
        if (TryComp<InteractionComponent>(ent, out var interactionComponent))
        {
            interactionComponent.IsEnabled = true;
        }
        if (TryComp<InputMoverComponent>(ent, out var inputMoverComponent))
        {
            inputMoverComponent.IsEnabled = true;
        }
    }

    private void SetDialogText(Entity<DialogContainerComponent> ent, string text)
    {
        ent.Comp.TextQueue = text;
    }

    private char NextDialogLetter(Entity<DialogContainerComponent> ent)
    {
        if (ent.Comp.TextQueue == null) return ' ';
        var a = ent.Comp.TextQueue[0];
        ent.Comp.TextQueue = ent.Comp.TextQueue.Substring(1);

        return a;
    }

    public void CleanupDialog(Entity<DialogContainerComponent> ent)
    {
        _dialogUiController.ClearDialogs();
        ent.Comp.DialogQueue.Clear();
        ent.Comp.TextQueue = null;
    }

    public void SetEmote(Texture? texture)
    {
        _dialogUiController.SetEmote(texture);
    }
    
    public void ContinueDialog(Entity<DialogContainerComponent> ent)
    {
        var comp = ent.Comp;
        
        if (comp.DialogQueue.Count == 0)
        {
            Hide(ent);
            return;
        }

        LoadLocation(ent);
        SetTitle(ent);
        SetDialogText(ent, comp.CurrentDialog.Text);
        EnsureDialogs(ent);
        EnsureChoices(ent);
        ShowCharacters(ent);
        HideCharacters(ent);
        
        if(comp.CurrentDialog.Character != null)
        {
            comp.SelectedCharacter = comp.CurrentDialog.Character.ToString();
        }
        
        if(!_characterSystem.TryGetCharacter(ent, comp.SelectedCharacter, out _, out var characterUid))
        {
           return;
        }
        
        if (comp.CurrentDialog.Name == null && comp.SelectedCharacter != null)
        {
            comp.CurrentDialog.Name = MetaData(characterUid).EntityName;
        }

        if (comp.CurrentDialog.Name != null && _dialogUiController.IsEmpty())
        {
            _dialogUiController.AppendLabel($"[bold]{comp.CurrentDialog.Name}[/bold]: ");
        }

        var startedEv = new DialogStartedEvent(comp.CurrentDialog, ent);

        if (characterUid.IsValid())
            RaiseLocalEvent(characterUid, startedEv);
        RaiseLocalEvent(ent, startedEv);
    }
    

    public override void FrameUpdate(float frameTime)
    {
        base.FrameUpdate(frameTime);

        var query = EntityQueryEnumerator<DialogContainerComponent>();
        while (query.MoveNext(out var uid, out var dialogComponent))
        {
            if(dialogComponent.DialogQueue.Count == 0 || dialogComponent.TextQueue == null) return;
        
            var ent = new Entity<DialogContainerComponent>(uid, dialogComponent);
            
            if (string.IsNullOrEmpty(dialogComponent.TextQueue))
            {
                dialogComponent.TextQueue = null;
                RaiseLocalEvent(uid, new DialogEndedEvent(dialogComponent.CurrentDialog, ent));
                return;
            }

            if (dialogComponent.CurrentDialog.PassedTime < dialogComponent.CurrentDialog.Delay)
            {
                dialogComponent.CurrentDialog.PassedTime += frameTime * 1000;
                return;
            }
        
            dialogComponent.CurrentDialog.PassedTime = 0;

            if (_characterSystem.TryGetCharacter(uid, dialogComponent.SelectedCharacter, out _, out var characterUid))
            {
                RaiseLocalEvent(characterUid,new DialogAppendEvent(dialogComponent.CurrentDialog, ent));
            }
        
            _dialogUiController.AppendLetter(NextDialogLetter(new Entity<DialogContainerComponent>(uid, dialogComponent)));
        }
    }
    
    private bool IsEmptyString(string text)
    {
        return string.IsNullOrEmpty(text) || text == " ";
    }
}

public sealed class SkipDialogHandler : InputCmdHandler
{
    private readonly DialogSystem _dialogSystem;

    public SkipDialogHandler(DialogSystem dialogSystem)
    {
        _dialogSystem = dialogSystem;
    }

    public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
    {
        if (session?.AttachedEntity is null || message.State == BoundKeyState.Down) return false;
        _dialogSystem.SkipMessage(_dialogSystem.EnsureDialogComp(session));
        return false;
    }
}
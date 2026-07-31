using System.Linq;
using System.Text;
using Content.Client.Camera.Systems;
using Content.Client.Character.Systems;
using Content.Client.Dialog.Components;
using Content.Client.Dialog.Data;
using Content.Client.GameVariables;
using Content.Client.Input;
using Content.Client.Interaction.Components;
using Content.Client.Location.Systems;
using Content.Client.Movement;
using Content.Client.Scene.Systems;
using Content.Client.UserInterface.Systems.Dialog;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.Player;

namespace Content.Client.Dialog.Systems;

public sealed partial class DialogSystem : EntitySystem
{
    [Dependency] private AnimationPlayerSystem _animationPlayerSystem = default!;
    [Dependency] private CameraSystem _cameraSystem = default!;
    [Dependency] private CharacterSystem _characterSystem = default!;
    [Dependency] private IClyde _clyde = default!;
    [Dependency] private LocationSystem _location = default!;
    [Dependency] private IUserInterfaceManager _userInterfaceManager = default!;
    [Dependency] private VariableManager _variableManager = default!;
    
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
        if (commonSession is null)
            throw new NullReferenceException("CommonSession is null");
        if (!commonSession.AttachedEntity.HasValue)
            throw new NullReferenceException("AttachedEntity is null");
        var entityUid = commonSession.AttachedEntity.Value;
        if (!TryComp<DialogContainerComponent>(entityUid, out var component))
            throw new InvalidOperationException("Did not find component of type DialogContainerComponent");
        return new Entity<DialogContainerComponent>(entityUid, component);
    }

    private void OnDialogEnd(Entity<DialogContainerComponent> ent, ref DialogEndedEvent ev)
    {
        if(ev.Dialog.Action is not null)
            ev.Dialog.Action.Act(IoCManager.Instance!, ent);
        
        if (!string.IsNullOrEmpty(ev.Dialog.Text?.ToString()))
        {
            if (ev.Dialog.SkipDialog)
            {
                ContinueDialog(ent);
                return;
            }

            if(ev.Dialog.Choices.Count == 0)
                _dialogUiController.AddButton(new DialogButton(){ Name = Loc.GetString("dialog-continue") }, ent);
            
            foreach (var choise in ev.Dialog.Choices) 
                _dialogUiController.AddButton(new DialogButton(){Name = choise.Key, Dialog = choise.Value.ToArray()}, ent);
            
            ent.Comp.CurrentDialog = null;
        }
        else
        {
            ContinueDialog(ent);
        }
    }
    
    public void SetDialog(Entity<DialogContainerComponent> ent, List<Data.Dialog> dialog)
    {
        CleanupDialog(ent);
        
        if(dialog.Count == 0)
             return;
        
        ent.Comp.RootContainer.Dialogs.AddRange(dialog);
    }
    
    public void AppendDialog(Entity<DialogContainerComponent> ent, List<Data.Dialog> dialog)
    {
        if(dialog.Count == 0)
            return;
        
        var container = new DialogContainer();
        container.Dialogs.AddRange(dialog);
        ent.Comp.RootContainer.EnqueueContainer(container);
    }

    public void SpeedupDialog(Entity<DialogContainerComponent> ent)
    {
        if (ent.Comp.CurrentDialog == null || ent.Comp.CurrentDialog.DontLetSkip) return;
        ent.Comp.CurrentMessageDelay = 2;
    }

    public void SkipMessage(Entity<DialogContainerComponent> ent)
    {
        if (ent.Comp.TextQueue.Count != 0)
        {
            SpeedupDialog(ent);
        }
        else
        {
            var btns = _dialogUiController.GetDialogButtons();
            if (btns.Count == 1) ActButton(ent, btns[0]);
        }
    }

    public void ActButton(Entity<DialogContainerComponent> ent, DialogButton button)
    {
        if (button.Dialog is null)
        {
            ContinueDialog(ent);
            return;
        }
        
        AppendDialog(ent, button.Dialog.ToList());
        ContinueDialog(ent);
    }

    private void Show(Entity<DialogContainerComponent> ent)
    {
        _dialogUiController.Show();
        ent.Comp.IsDialogVisible = true;
        if (TryComp<InteractionComponent>(ent, out var interactionComponent)) interactionComponent.IsEnabled = false;
        if (TryComp<InputMoverComponent>(ent, out var inputMoverComponent)) inputMoverComponent.IsMoveEnabled = false;
    }

    private void Hide(Entity<DialogContainerComponent> ent)
    {
        _dialogUiController.Hide();
        ent.Comp.IsDialogVisible = false;
        if (TryComp<InteractionComponent>(ent, out var interactionComponent)) interactionComponent.IsEnabled = true;
        if (TryComp<InputMoverComponent>(ent, out var inputMoverComponent)) inputMoverComponent.IsMoveEnabled = true;
    }

    private void SetDialogText(Entity<DialogContainerComponent> ent, string text)
    {
        foreach (var ch in text)
        {
            ent.Comp.TextQueue.Enqueue(ch);
        }
    }

    private char NextDialogLetter(Entity<DialogContainerComponent> ent)
    {
        if (!ent.Comp.TextQueue.TryDequeue(out var letter)) return ' ';
        
        if (letter != '{') 
            return letter;
        
        var cmd = new StringBuilder();
        while (ent.Comp.TextQueue.TryDequeue(out letter) && letter != '}')
        {
            cmd.Append(letter);
        }

        ent.Comp.TextQueue.TryDequeue(out letter);

        if (!int.TryParse(cmd.ToString(), out var newDelay)) 
            return letter;
        
        ent.Comp.CurrentMessageDelay = newDelay;
        return letter;
    }

    public void CleanupDialog(Entity<DialogContainerComponent> ent)
    {
        _dialogUiController.ClearDialogs();
        ent.Comp.RootContainer.Clear();
        ent.Comp.TextQueue.Clear();
    }

    public void SetEmote(Texture? texture)
    {
        _dialogUiController.SetEmote(texture);
    }

    public void ContinueDialog(Entity<DialogContainerComponent> ent)
    {
        var comp = ent.Comp;

        if (comp.RootContainer.IsEmpty())
        {
            Hide(ent);
            return;
        }

        if(!comp.IsDialogVisible)
        {
            Show(ent);
        }

        var dialog = comp.RootContainer.Dequeue()!;
        comp.CurrentDialog = dialog;
        comp.CurrentMessageDelay = dialog.Delay;
        
        _dialogUiController.ClearButtons();
        
        if (dialog.StopDialog)
        {
            CleanupDialog(ent);
            Hide(ent);
            return;
        }
        
        CheckTweaksOfText(ent, dialog);
        LoadLocation(ent, dialog);
        SetTitle(ent, dialog);
        if (dialog.Text != null) 
            SetDialogText(ent, dialog.Text);
        EnsureDialogs(ent, dialog);
        ShowCharacters(ent, dialog);
        HideCharacters(ent, dialog);

        if (dialog.Character != null) comp.SelectedCharacter = dialog.Character.ToString();

        if (!_characterSystem.TryGetCharacter(ent, comp.SelectedCharacter, out _, out var characterUid)) return;

        if (dialog.Name == null && comp.SelectedCharacter != null)
            dialog.Name = MetaData(characterUid).EntityName;

        if (dialog.Name != null && _dialogUiController.IsEmpty())
            _dialogUiController.AppendLabel($"[bold]{dialog.Name}[/bold]: ");
        
        if (dialog.Variable is not null)
        {
            var variable = dialog.Variable;
            _variableManager.Set(variable.Name, variable.Value);
        }

        if (!string.IsNullOrEmpty(dialog.Set))
        {
            _variableManager.ParseAsObject(dialog.Set);
        }

        if (!string.IsNullOrEmpty(dialog.If))
        {
            var statement = CheckObj(_variableManager.ParseAsObject(dialog.If));

            if (statement && dialog.Then is not null)
                AppendDialog(ent, dialog.Then.ToList());
            if (!statement && dialog.Else is not null)
                AppendDialog(ent, dialog.Else.ToList());
        }

        var startedEv = new DialogStartedEvent(dialog, ent);

        if (characterUid.IsValid())
            RaiseLocalEvent(characterUid, startedEv);
        RaiseLocalEvent(ent, startedEv);

        if (dialog.Scene is not null)
            EntityManager.System<SceneSystem>().LoadScene(ent, dialog.Scene.Value);
    }

    private bool CheckObj(object? value)
    {
        return value is not null && ((value is double d && d == 1) || (value is bool f && f));
    }

    public override void FrameUpdate(float frameTime)
    {
        base.FrameUpdate(frameTime);

        var query = EntityQueryEnumerator<DialogContainerComponent>();
        while (query.MoveNext(out var uid, out var dialogComponent))
        {
            if (dialogComponent.CurrentDialog is null) continue;
            
            var ent = new Entity<DialogContainerComponent>(uid, dialogComponent);

            if (dialogComponent.TextQueue.Count == 0)
            {
                RaiseLocalEvent(uid, new DialogEndedEvent(dialogComponent.CurrentDialog, ent));
                continue;
            }
            
            if (dialogComponent.PassedTime < dialogComponent.CurrentMessageDelay)
            {
                dialogComponent.PassedTime += frameTime * 1000;
                continue;
            }

            dialogComponent.PassedTime = 0;

            if (_characterSystem.TryGetCharacter(uid, dialogComponent.SelectedCharacter, out _, out var characterUid))
                RaiseLocalEvent(characterUid, new DialogAppendEvent(dialogComponent.CurrentDialog, ent));

            _dialogUiController.AppendLetter(
                NextDialogLetter(new Entity<DialogContainerComponent>(uid, dialogComponent)));
        }
    }
}

public sealed class SkipDialogHandler : InputCmdHandler
{
    private readonly DialogSystem _dialogSystem;

    public SkipDialogHandler(DialogSystem dialogSystem)
    {
        _dialogSystem = dialogSystem;
    }

    public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session,
        IFullInputCmdMessage message)
    {
        if (session?.AttachedEntity is null || message.State == BoundKeyState.Down) return false;
        _dialogSystem.SkipMessage(_dialogSystem.EnsureDialogComp(session));
        return false;
    }
}
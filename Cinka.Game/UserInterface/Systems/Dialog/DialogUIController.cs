using System;
using System.Collections.Generic;
using Cinka.Game.Character.Components;
using Cinka.Game.Character.Managers;
using Cinka.Game.Dialog.Data;
using Cinka.Game.Gameplay;
using Cinka.Game.Input;
using Cinka.Game.UserInterface.Systems.Dialog.Widgets;
using Robust.Client.GameObjects;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Timing;

namespace Cinka.Game.UserInterface.Systems.Dialog;

public sealed class DialogUIController : UIController
{
    [Dependency] private readonly IInputManager _input = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly ICharacterManager _characterManager = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    
    private readonly HashSet<DialogGui> _dialogGuis = new();
    private readonly List<Game.Dialog.Data.Dialog> _messageQueue = new();

    public bool IsMessage => _messageQueue.Count > 0;

    public event Action<Game.Dialog.Data.Dialog>? MessageEnded;

    public override void Initialize()
    {
        base.Initialize();

        var gameplayStateLoad = UIManager.GetUIController<GameplayStateLoadController>();
        gameplayStateLoad.OnScreenLoad += OnScreenLoad;
        gameplayStateLoad.OnScreenUnload += OnScreenUnload;
        
        var cmdhandler = InputCmdHandler.FromDelegate(_ =>
            SkipMessage());
        
        _input.SetInputCommand(ContentKeyFunctions.SkipDialog,cmdhandler);
        _input.SetInputCommand(EngineKeyFunctions.UIClick,cmdhandler);
    }

    private void OnScreenLoad()
    {
    }

    private void OnScreenUnload()
    {
    }

    public void RegisterDialog(DialogGui dialogGui)
    {
        _dialogGuis.Add(dialogGui);
    }

    public void UnregisterDialog(DialogGui dialogGui)
    {
        _dialogGuis.Remove(dialogGui);
    }

    public void AppendText(Game.Dialog.Data.Dialog dialog)
    {
        _messageQueue.Add(dialog);
    }

    public void SpeedUpText()
    {
        if (!IsMessage || _messageQueue[0].DontLetSkip)
            return;

        _messageQueue[0].Delay = 10;
    }

    public void SkipMessage()
    {
        if(IsMessage) SpeedUpText();
        else
        {
            foreach (var dialog in _dialogGuis)
            {
                dialog.InvokeButton();
            }
        }
    }

    public override void FrameUpdate(FrameEventArgs args)
    {
        if (!IsMessage)
            return;

        var currentDialog = _messageQueue[0];

        if (string.IsNullOrEmpty(currentDialog.Text))
        {
            _messageQueue.RemoveAt(0);
            OnMessageEnded(currentDialog);
            return;
        }

        currentDialog.PassedTime += args.DeltaSeconds * 1000;

        if (currentDialog.PassedTime >= currentDialog.Delay)
        {
            currentDialog.PassedTime = 0;
            foreach (var dialogGui in _dialogGuis)
            {
                //TODO: Вынести бы это как-то в отдельный метод
                if (dialogGui.IsEmpty)
                {
                    if(!string.IsNullOrEmpty(currentDialog.Name) )
                        dialogGui.AppendLabel($"[bold]{currentDialog.Name}[/bold]: ");
                    
                    //TODO: переделать потом да
                    if (currentDialog.Character != null 
                        && _characterManager.TryGetCharacter(currentDialog.Character, out var characterData) 
                        && _entityManager.TryGetComponent<EmoteComponent>(characterData.Uid,out var emoteComponent)
                        && StaticIoC.ResC.TryGetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / emoteComponent.RsiPath,
                            out var rs)
                        && rs.RSI.TryGetState(currentDialog.Emote, out var state))
                    {
                        dialogGui.SetEmote(state.Frame0);
                    }
                    else if (currentDialog.IsDialog)
                    {
                        dialogGui.SetEmote(null);
                    }
                }

                dialogGui.AppendLetter(currentDialog.Text[0]);
                currentDialog.Text = currentDialog.Text.Substring(1);
            }
        }
    }

    public void ClearAllDialog()
    {
        foreach (var dialog in _dialogGuis) dialog.ClearText();
    }

    public void AddButton(DialogButton button)
    {
        foreach (var dialog in _dialogGuis) dialog.AddButton(button);
    }

    private void OnMessageEnded(Game.Dialog.Data.Dialog obj)
    {
        MessageEnded?.Invoke(obj);
    }
}
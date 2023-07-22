using System;
using System.Collections.Generic;
using Cinka.Game.Dialog.Data;
using Cinka.Game.UserInterface.Systems.Dialog.Widgets;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Timing;

namespace Cinka.Game.UserInterface.Systems.Dialog;

public sealed class DialogUIController : UIController
{

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
        if(!IsMessage)
            return;

        _messageQueue[0].Delay = 10;
    }

    public override void FrameUpdate(FrameEventArgs args)
    {
        if(!IsMessage)
            return;
        
        var currentDialog = _messageQueue[0];

        if (string.IsNullOrEmpty(currentDialog.Text))
        {
            OnMessageEnded(currentDialog);
            _messageQueue.RemoveAt(0);
            return;
        }

        currentDialog.PassedTime += args.DeltaSeconds * 1000;

        if (currentDialog.PassedTime >= currentDialog.Delay)
        {
            currentDialog.PassedTime = 0;
            foreach (var dialogGui in _dialogGuis)
            {
                dialogGui.AppendLetter(currentDialog.Text[0]);
                currentDialog.Text = currentDialog.Text.Substring(1);
            }
        }


    }

    public void ClearAllDialog()
    {
        foreach (var dialog in _dialogGuis)
        {
            dialog.ClearText();
        }
    }

    public void AddButton(DialogButton button)
    {
        foreach (var dialog in _dialogGuis)
        {
            dialog.AddButton(button);
        }
    }

    private void OnMessageEnded(Game.Dialog.Data.Dialog obj)
    {
        MessageEnded?.Invoke(obj);
    }
}



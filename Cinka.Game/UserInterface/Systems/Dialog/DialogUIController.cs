using System;
using System.Collections.Generic;
using Cinka.Game.UserInterface.Systems.Dialog.Widgets;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Cinka.Game.UserInterface.Systems.Dialog;

public sealed class DialogUIController : UIController
{

    private readonly HashSet<DialogGui> _dialogGuis = new();
    private readonly List<Dialog> _messageQueue = new();

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

    public void AppendText(Dialog dialog)
    {
        _messageQueue.Add(dialog);
    }

    public override void FrameUpdate(FrameEventArgs args)
    {
        if(_messageQueue.Count == 0)
            return;
        
        var currentDialog = _messageQueue[0];

        if (string.IsNullOrEmpty(currentDialog.Text))
        {
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
}

[Serializable,NetSerializable]
public sealed class Dialog
{
    public string Text;
    public float Delay;
    public float PassedTime = 0;

    public Dialog(string text, float delay)
    {
        Text = text;
        Delay = delay;
    }
}
using System;
using System.Collections.Generic;
using Content.Game.Character;
using Content.Game.Dialog.Data;
using Content.Game.UserInterface.Systems.Dialog.Widgets;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controllers;

namespace Content.Game.UserInterface.Systems.Dialog;

public sealed class DialogUIController : UIController
{
    private DialogGui? _dialogGui;

    public void RegisterDialog(DialogGui dialogGui)
    {
        _dialogGui = dialogGui;
    }

    public void UnregisterDialog(DialogGui dialogGui)
    {
        _dialogGui = null;
    }

    public void SetEmote(Texture? texture)
    {
        _dialogGui?.SetEmote(texture);
    }
    
    public void AppendLabel(string text)
    {
        _dialogGui?.AppendLabel(text);
    }

    public void AppendLetter(char letter)
    {
        _dialogGui?.AppendLetter(letter);
    }

    public void ClearDialogs()
    {
        _dialogGui?.ClearText();
    }

    public bool IsEmpty()
    {
        return _dialogGui == null || _dialogGui.IsEmpty();
    }

    public void AddButton(DialogButton button)
    {
        _dialogGui?.AddButton(button);
    }

    public List<DialogButton> GetDialogButtons()
    {
        return _dialogGui?.Buttons != null ? _dialogGui.Buttons : [];
    }

    public void Hide()
    {
        if (_dialogGui != null) _dialogGui.Visible = false;
        CharacterRenderingOverlay.IsVisible = false;
    }
    
    public void Show()
    {
        if (_dialogGui != null) _dialogGui.Visible = true;
        CharacterRenderingOverlay.IsVisible = true;
    }
}
using System.Collections.Generic;
using Cinka.Game.Dialog.Data;
using Cinka.Game.UserInterface.Systems.Dialog;
using JetBrains.Annotations;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Dialog.DialogActions;

[UsedImplicitly]
public sealed class LoadButtons : IDialogAction
{
    [DataField("buttons")]
    public HashSet<DialogButton> Buttons = new();
    public void Act()
    {
        var controller = IoCManager.Resolve<IUserInterfaceManager>().GetUIController<DialogUIController>();
        foreach (var button in Buttons)
        {
            controller.AddButton(button);
        }
    }
}
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Cinka.Game.UserInterface.Systems.Dialog.Widgets;

public sealed partial class DialogGui : UIWidget
{
    public DialogGui()
    {
        RobustXamlLoader.Load(this);
        
    }
}
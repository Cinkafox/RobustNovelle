namespace Content.Client.Dialog.Components;

[RegisterComponent]
public sealed partial class DialogContainerComponent : Component
{
    [ViewVariables] public readonly List<Data.Dialog> DialogQueue = [];
    
    [ViewVariables] public string? TextQueue = null;

    [ViewVariables] public bool HasDialog => DialogQueue.Count > 0;
    
    [ViewVariables] public Dialog.Data.Dialog CurrentDialog => DialogQueue[0];
}
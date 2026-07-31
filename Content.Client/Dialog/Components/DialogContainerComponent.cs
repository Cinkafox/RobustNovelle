using System.Linq;
using Robust.Shared.Prototypes;

namespace Content.Client.Dialog.Components;

[RegisterComponent]
public sealed partial class DialogContainerComponent : Component
{
    [ViewVariables] public readonly DialogContainer RootContainer = new DialogContainer();
    [ViewVariables] public EntProtoId? CameraFollowProtoId;
    [ViewVariables] public EntProtoId? SelectedCharacter;
    
    [ViewVariables] public Queue<char> TextQueue = [];
    [ViewVariables] public bool IsDialogVisible;
    [ViewVariables] public Data.Dialog? CurrentDialog;
    [ViewVariables] public float CurrentMessageDelay = 30;
    
    [ViewVariables(VVAccess.ReadOnly)] public float PassedTime;
}

[DataDefinition]
public sealed partial class DialogContainer
{
    [DataField] public List<DialogContainer> ChildContainers = [];
    [DataField] public List<Data.Dialog> Dialogs = [];

    public DialogContainer DequeueContainer()
    {
        if(ChildContainers.Count == 0) return this;
        var container = ChildContainers[0];
        ChildContainers.RemoveAt(0);
        return container;
    }

    public void EnqueueContainer(DialogContainer container)
    {
        ChildContainers.Insert(0, container);
    }

    private DialogContainer? GetNonEmptyContainer()
    {
        foreach (var container in ChildContainers.ToList())
        {
            var childContainer = container.GetNonEmptyContainer();
            if (childContainer == null) 
            {
                ChildContainers.Remove(container);
                continue;
            }
            
            return childContainer;
        }

        if(Dialogs.Count == 0) return null;
        return this;
    }

    public Data.Dialog? Dequeue()
    {
        var container = GetNonEmptyContainer();
        if(container == null) return null;
        
        var currDialog = container.Dialogs[0];
        container.Dialogs.RemoveAt(0);
        return currDialog;
    }

    public void Clear()
    {
        ChildContainers.Clear();
        Dialogs.Clear();
    }

    public bool IsEmpty()
    {
        return GetNonEmptyContainer() is null;
    }
}
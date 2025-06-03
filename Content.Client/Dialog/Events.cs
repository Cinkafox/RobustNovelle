using Content.Client.Dialog.Components;

namespace Content.Client.Dialog;

[Serializable]
public abstract class BaseDialogEvent(Data.Dialog dialog, Entity<DialogContainerComponent> dialogEntity) : EntityEventArgs
{
    public Entity<DialogContainerComponent> DialogEntity = dialogEntity;
    public Data.Dialog Dialog = dialog;
}

public sealed class DialogStartedEvent(Data.Dialog dialog, Entity<DialogContainerComponent> dialogEntity) : BaseDialogEvent(dialog, dialogEntity);

public sealed class DialogAppendEvent(Data.Dialog dialog, Entity<DialogContainerComponent> dialogEntity) : BaseDialogEvent(dialog, dialogEntity);

public sealed class DialogEndedEvent(Data.Dialog dialog, Entity<DialogContainerComponent> dialogEntity) : BaseDialogEvent(dialog, dialogEntity);
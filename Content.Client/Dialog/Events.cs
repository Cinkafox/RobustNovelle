using System;
using Robust.Shared.GameObjects;

namespace Content.Client.Dialog;

[Serializable]
public abstract class BaseDialogEvent(Data.Dialog dialog) : EntityEventArgs
{
    public Data.Dialog Dialog = dialog;
}

public sealed class DialogStartedEvent(Data.Dialog dialog) : BaseDialogEvent(dialog);

public sealed class DialogAppendEvent(Data.Dialog dialog) : BaseDialogEvent(dialog);

public sealed class DialogEndedEvent(Data.Dialog dialog) : BaseDialogEvent(dialog);
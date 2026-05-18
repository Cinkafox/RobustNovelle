using Robust.Shared.Prototypes;

namespace Content.Client.Dialog.Components;

[RegisterComponent]
public sealed partial class DialogContainerComponent : Component
{
    [ViewVariables] public readonly List<Data.Dialog> DialogQueue = [];
    [ViewVariables] public EntProtoId? CameraFollowProtoId;

    [ViewVariables] public EntProtoId? SelectedCharacter;

    [ViewVariables] public string? TextQueue = null;

    [ViewVariables] public bool HasDialog => DialogQueue.Count > 0;

    [ViewVariables] public Data.Dialog CurrentDialog => DialogQueue[0];
}
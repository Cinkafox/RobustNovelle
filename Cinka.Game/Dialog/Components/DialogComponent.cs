using Cinka.Game.Dialog.Data;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Dialog.Components;

[RegisterComponent]
public sealed partial class DialogComponent : Component
{
    [DataField] public ProtoId<DialogPrototype>? Dialog;
}
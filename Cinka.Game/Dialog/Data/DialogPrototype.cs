using Robust.Shared.Prototypes;

namespace Cinka.Game.Dialog.Data;

[Prototype("dialogStyle")]
public sealed class DialogPrototype : DialogStyle , IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;
}
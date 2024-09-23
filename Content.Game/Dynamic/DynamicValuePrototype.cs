using Robust.Shared.Prototypes;

namespace Content.Game.Dynamic;

[Prototype("dynamicValue")]
public sealed class DynamicValuePrototype: IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField] public Game.Dynamic.DynamicValue Value = default!;
}
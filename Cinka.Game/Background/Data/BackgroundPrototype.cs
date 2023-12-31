using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Background.Data;

[Prototype("background")]
public sealed class BackgroundPrototype : IPrototype
{
    [IdDataField] public string ID { get; private set; } = default!;
    
    [DataField(readOnly: true)] public List<PrototypeLayerData> Layers = new();

    [DataField("sprite")] public string? RsiPath;

    [DataField] public string? State;
    
}
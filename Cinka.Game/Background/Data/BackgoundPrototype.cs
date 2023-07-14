using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Background.Data;

[Prototype("background")]
public sealed class BackgoundPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; }
    
    [DataField("sprite")]
    public string? RsiPath;
    
    [DataField("state")]
    public string? State;
    
    [DataField("layers", readOnly: true)] public List<PrototypeLayerData> Layers = new();
}
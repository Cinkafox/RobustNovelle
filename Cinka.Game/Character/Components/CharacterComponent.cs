using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Character.Components;

[RegisterComponent]
public sealed class CharacterComponent : Component
{
    [DataField("sprite")]
    public string RsiPath = string.Empty;
    
    [DataField("state")]
    public string State = "default";
    
    [DataField("layers", readOnly: true)] public List<PrototypeLayerData> Layers = new();
}
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Character.Components;

[RegisterComponent]
public sealed partial class CharacterComponent : Component
{
    [DataField(readOnly: true)] public List<PrototypeLayerData> Layers = new();

    [DataField("sprite")] public string RsiPath = string.Empty;

    [DataField] public string State = "default";
}
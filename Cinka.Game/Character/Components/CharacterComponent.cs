using System.Collections.Generic;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Cinka.Game.Character.Components;

[RegisterComponent]
public sealed partial class CharacterComponent : Component
{
    [DataField(readOnly: true)] public List<PrototypeLayerData> Layers = new();

    [DataField("sprite")] public string RsiPath = string.Empty;

    [DataField] public string State = "default";
    
    [DataField] public bool IsVisible;
    
    [DataField("scale")] public Vector2 Scale = Vector2.One;

    [ViewVariables] public RSI Sprite;
}
using System.Collections.Generic;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Character.Components;

[RegisterComponent]
public sealed partial class CharacterComponent : Component
{
    [DataField(readOnly: true)] public List<PrototypeLayerData> Layers = new();

    [DataField("sprite")] public string RsiPath = string.Empty;

    [DataField] public string State = "default";
    [DataField] public bool Visible;
    
    [DataField] public Vector2 Scale = Vector2.One;
    [DataField, Animatable] public double XPosition { get; set; } = -1;

    [ViewVariables] public RSI Sprite;
}
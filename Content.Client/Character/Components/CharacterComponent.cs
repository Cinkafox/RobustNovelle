using System.Numerics;
using Robust.Client.Graphics;
using Robust.Shared.Animations;

namespace Content.Client.Character.Components;

[RegisterComponent]
public sealed partial class CharacterComponent : Component
{
    [DataField(readOnly: true)] public List<PrototypeLayerData> Layers = new();

    [DataField("sprite")] public string RsiPath = string.Empty;

    [DataField] public Vector2 Scale = Vector2.One;

    [ViewVariables] public RSI Sprite;

    [DataField] public string State = "default";
    [DataField] public bool Visible;
    [DataField] [Animatable] public double XPosition { get; set; } = -1;
}
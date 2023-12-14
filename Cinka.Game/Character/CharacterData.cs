using System.Numerics;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Cinka.Game.Character;

[DataDefinition]
public sealed partial class CharacterData
{
    [DataField] public bool Visible = true;
    
    [DataField("scale")] public Vector2 Scale = Vector2.One;

    [ViewVariables] public RSI Sprite;

    [DataField("state")] public string State = "default";

    [ViewVariables] public EntityUid Uid;

    public CharacterData(RSI sprite)
    {
        Sprite = sprite;
    }
}
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Character.Components;

[RegisterComponent]
public sealed partial class EmoteComponent : Component
{
    [DataField("sprite")] public string RsiPath = string.Empty;
}
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Character.Components;

[RegisterComponent]
public sealed partial class EmoteComponent : Component
{
    [DataField("sprite")] public string RsiPath = string.Empty;
}
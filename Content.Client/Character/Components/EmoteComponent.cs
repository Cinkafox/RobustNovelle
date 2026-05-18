namespace Content.Client.Character.Components;

[RegisterComponent]
public sealed partial class EmoteComponent : Component
{
    [DataField] public string Default = "default";
    [DataField("sprite")] public string RsiPath = string.Empty;
}
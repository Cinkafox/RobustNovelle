namespace Content.Client.Character.Components;

[RegisterComponent]
public sealed partial class EmoteComponent : Component
{
    [DataField("sprite")] public string RsiPath = string.Empty;
    [DataField] public string Default = "default";
}
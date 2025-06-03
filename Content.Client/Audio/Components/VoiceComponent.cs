using Robust.Shared.Audio;

namespace Content.Client.Audio.Components;

[RegisterComponent]
public sealed partial class VoiceComponent : Component
{
    [DataField] public SoundSpecifier Voice;
}
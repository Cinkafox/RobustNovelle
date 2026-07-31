using Robust.Shared.Audio;

namespace Content.Client.Audio.Components;

[RegisterComponent]
public sealed partial class VoiceComponent : Component
{
    [DataField] public SoundSpecifier Voice;
    [DataField] public TimeSpan MinVoiceTimeSpan = TimeSpan.FromMilliseconds(50);
    [ViewVariables] public TimeSpan LastVoiceTime;
}
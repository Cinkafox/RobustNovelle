using Content.Client.Audio.Data;
using Robust.Shared.Prototypes;

namespace Content.Client.Audio.Components;

[RegisterComponent]
public sealed partial class VoiceComponent : Component
{
    [DataField] public ProtoId<AudioPrototype> Voice;
}
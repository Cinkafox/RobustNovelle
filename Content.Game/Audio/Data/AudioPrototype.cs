using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Game.Audio.Data;

[Prototype("audio")]
public sealed class AudioPrototype : IPrototype
{
    [IdDataField] public string ID { get; } = default!;

    [DataField] public bool IsBackground;

    [DataField(required:true)] //,customTypeSerializer:typeof(SoundSpecifierTypeSerializer))] 
    public SoundSpecifier Audio = default!;
}
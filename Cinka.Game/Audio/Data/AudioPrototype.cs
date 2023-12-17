using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Audio.Data;

[Prototype("audio")]
public sealed class AudioPrototype : IPrototype
{
    [IdDataField] public string ID { get; } = default!;

    //[DataField(required: true)] public string Path = default!;

    [DataField] public bool IsBackground;

    [DataField("path", required:true, customTypeSerializer:typeof(SoundSpecifierTypeSerializer))] 
    public SoundSpecifier Audio = default!;
}
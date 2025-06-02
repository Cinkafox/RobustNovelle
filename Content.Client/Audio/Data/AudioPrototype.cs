using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Client.Audio.Data;

[Prototype("audio")]
public sealed class AudioPrototype : IPrototype
{
    [IdDataField] public string ID { get; } = default!;
    
    [DataField] public AudioParams AudioParams { get; set; }
    [DataField] public ProtoId<AudioPresetPrototype>? Effect { get; set; }
    [DataField] public AudioRepeatOptions? Repeat { get; set; } 

    [DataField(required:true)] public SoundSpecifier Audio { get; set; } = default!;
}

[DataDefinition]
public partial struct AudioRepeatOptions
{
    [DataField] public HashSet<int> Intervals { get; set; } = [1];
}
using Content.Client.Dialog.Data;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Client.Location.Data;

[Prototype]
public sealed partial class LocationPrototype : IPrototype
{
    [IdDataField] public string ID { get; private set; } = default!;
    
    [DataField] public HashSet<SoundSpecifier> AmbientSounds = [];
    [DataField] public HashSet<EntityDefinition> Entities = [];
    
    [DataField(required:true)] public ILocationDefinition Location = default!;
}
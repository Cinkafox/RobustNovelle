using System.IO;
using Robust.Client.ResourceManagement;
using Robust.Shared.Audio;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;
using static Cinka.Game.StaticIoC;

namespace Cinka.Game.Audio.Data;

[Prototype("audio")]
public sealed class AudioPrototype : IPrototype
{
    [IdDataField] public string ID { get; } = default!;

    //[DataField(required: true)] public string Path = default!;

    [DataField] public bool IsBackground = false;

    [DataField("path", required:true, customTypeSerializer:typeof(AudioSerializer))] 
    public AudioSpecifier Audio = default!;
}
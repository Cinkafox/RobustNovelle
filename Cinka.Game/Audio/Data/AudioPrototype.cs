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
public sealed class AudioPrototype : IPrototype, ISerializationHooks
{
    [IdDataField] public string ID { get; }

    [DataField("path", required: true)] public string Path = default!;

    [DataField("isBackground")] public bool IsBackground = false;

    [ViewVariables] public AudioResource AudioStream = default!;

    void ISerializationHooks.AfterDeserialization()
    {
        if (!ResC.TryGetResource<AudioResource>(new ResPath(Path), out AudioStream))
        {
            throw new FileNotFoundException($"Could not find a audio file: {Path}");
        }
    }
}
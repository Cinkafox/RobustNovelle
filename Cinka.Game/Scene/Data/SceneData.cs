using System.Collections.Generic;
using Cinka.Game.Location.Data;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Cinka.Game.Scene.Data;

[DataDefinition]
public sealed class SceneData
{
    [DataField("location", customTypeSerializer: typeof(PrototypeIdSerializer<LocationPrototype>))]
    public string LocationPrototype = "default";

    //customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))
    [DataField("characters")]
    public HashSet<string> Characters = new();

    [DataField("dialogs")]
    public List<Dialog.Data.Dialog> Dialogs = new();
}
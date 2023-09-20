using System.Collections.Generic;
using Cinka.Game.Location.Data;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Cinka.Game.Scene.Data;

[DataDefinition]
public sealed partial class SceneData
{
    //customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))
    [DataField] public HashSet<EntProtoId> Characters = new();

    [DataField] public List<Dialog.Data.Dialog> Dialogs = new();

    [DataField] public string Id = "";

    [DataField] public ProtoId<LocationPrototype> Location = "default";
}
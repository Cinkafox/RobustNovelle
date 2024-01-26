using System.Collections.Generic;
using Cinka.Game.Dialog.Data;
using Cinka.Game.Location.Data;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Scene.Data;

[Prototype("scene")]
public sealed class ScenePrototype : IPrototype
{
    [IdDataField] public string ID { get; private set; } = default!;
    
    [DataField] public HashSet<Character.Data.Character> Characters = new();

    [DataField] public ProtoId<LocationPrototype> Location = "default";

    [DataField] public ProtoId<DialogPrototype>? Dialog;

}
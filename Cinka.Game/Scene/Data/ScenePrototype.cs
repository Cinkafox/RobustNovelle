using System.Collections.Generic;
using Cinka.Game.Location.Data;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Scene.Data;

[Prototype("scene")]
public sealed class ScenePrototype : IPrototype
{
    [IdDataField] public string ID { get; private set; } = default!;
    
    [DataField] public HashSet<Character> Characters = new();

    [DataField] public List<Dialog.Data.Dialog> Dialogs = new();

    [DataField] public ProtoId<LocationPrototype> Location = "default";
    
}

[DataDefinition]
public sealed partial class Character
{
    [DataField] public EntProtoId Entity = new();
    [DataField] public bool Visible = true;
} 
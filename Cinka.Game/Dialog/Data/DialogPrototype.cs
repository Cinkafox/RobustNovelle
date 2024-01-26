using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Dialog.Data;

[Prototype("dialog")]
public sealed partial class DialogPrototype : IPrototype
{
    [IdDataField] public string ID { get; private set; } = default!;
    
    [DataField] public HashSet<EntProtoId> Characters = new();

    [DataField] public List<Dialog> Dialogs = new();
}

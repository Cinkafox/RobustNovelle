using System.Collections.Generic;
using Content.Game.Location.Data;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Game.Scene.Data;

[Prototype("scene")]
public sealed class ScenePrototype : IPrototype
{
    [IdDataField] public string ID { get; private set; } = default!;

    [DataField] public List<Dialog.Data.Dialog> Dialogs = new();
}
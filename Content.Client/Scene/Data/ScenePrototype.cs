using Robust.Shared.Prototypes;

namespace Content.Client.Scene.Data;

[Prototype("scene")]
public sealed class ScenePrototype : SceneData, IPrototype
{
    [IdDataField] public string ID { get; private set; } = default!;
}

[Virtual]
public class SceneData
{
    [DataField] public List<Dialog.Data.Dialog> Dialogs = new();
}
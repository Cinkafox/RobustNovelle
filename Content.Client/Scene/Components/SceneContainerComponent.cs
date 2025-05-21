using Content.Client.Scene.Data;

namespace Content.Client.Scene.Components;

[RegisterComponent]
public sealed partial class SceneContainerComponent : Component
{
    [DataField] public SceneData? CurrentScene;
}
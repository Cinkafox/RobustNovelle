using Robust.Client.Graphics;
using Robust.Shared.Utility;

namespace Content.Client.Interaction.Components;

[RegisterComponent]
public sealed partial class InteractionComponent : Component
{
    [DataField] public bool IsEnabled;
    [ViewVariables] public (InteractibleComponent, TransformComponent)? CurrentInteractible;
}
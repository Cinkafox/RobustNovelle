namespace Content.Client.Interaction.Components;

[RegisterComponent]
public sealed partial class InteractionComponent : Component
{
    [ViewVariables] public (InteractibleComponent, TransformComponent)? CurrentInteractible;
    [DataField] public bool IsEnabled;
}
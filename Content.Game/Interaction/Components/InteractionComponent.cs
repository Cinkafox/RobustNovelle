using Robust.Client.Graphics;
using Robust.Shared.Utility;

namespace Content.Game.Interaction.Components;

[RegisterComponent]
public sealed partial class InteractionComponent : Component
{
    [DataField] public float MaxDistance = 1f;
    [DataField] public bool IsEnabled = true;
    [DataField] public ResPath InteractionIconPath = new("/Textures/Interface/interaction.png");

    [ViewVariables] public Texture IconTexture;
    [ViewVariables] public (InteractibleComponent, TransformComponent)? CurrentInteractible;
}
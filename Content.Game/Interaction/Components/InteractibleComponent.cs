using Content.Game.Dialog.Data;

namespace Content.Game.Interaction.Components;

[RegisterComponent]
public sealed partial class InteractibleComponent : Component
{
    [DataField] public List<IDialogAction> Actions = new();
}
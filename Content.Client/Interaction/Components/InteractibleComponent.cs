using Content.Client.Dialog.Data;

namespace Content.Client.Interaction.Components;

[RegisterComponent]
public sealed partial class InteractibleComponent : Component
{
    [DataField] public List<IDialogAction> Actions = new();
}
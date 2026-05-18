using Content.Client.Dialog.Data;
using Content.Client.PasterString.Data;

namespace Content.Client.Interaction.Components;

[RegisterComponent]
public sealed partial class InteractibleComponent : Component
{
    [DataField] public List<IDialogAction> Actions = new();
    [DataField] public bool InvokeImmediately;
    [DataField] public float MaxDistance = 1f;
    [DataField] public SmartString Name = "Взаимодействовать";
}
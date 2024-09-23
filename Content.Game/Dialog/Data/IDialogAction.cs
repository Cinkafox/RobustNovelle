using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Game.Dialog.Data;

[ImplicitDataDefinitionForInheritors]
public partial interface IDialogAction
{
    void Act();
}
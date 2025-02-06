using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Dialog.Data;

[ImplicitDataDefinitionForInheritors]
public partial interface IDialogAction
{
    void Act();
}
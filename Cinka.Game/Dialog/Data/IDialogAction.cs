using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Dialog.Data;

[ImplicitDataDefinitionForInheritors]
public partial interface IDialogAction
{
    void Act();
}
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Dialog.Data;

[ImplicitDataDefinitionForInheritors]
public interface IDialogAction
{
    void Act();
}
using Content.Client.Dialog.Components;
using Content.Client.Dialog.Data;
using Content.Client.GameVariables;
using Content.StyleSheetify.Shared.Dynamic;

namespace Content.Client.Dialog.DialogActions;

[DataDefinition]
public sealed partial class ExecuteVariableAction : IDialogAction
{
    [DataField] public string Value;

    public void Act(IDependencyCollection collection, Entity<DialogContainerComponent> actorUid)
    {
        var variableSystem = collection.Resolve<VariableManager>();
        variableSystem.ParseAsObject(Value);
    }
}

[DataDefinition]
public sealed partial class SetVariableAction : IDialogAction
{
    [DataField] public string Name;
    [DataField] public DynamicValue Value;

    public void Act(IDependencyCollection collection, Entity<DialogContainerComponent> actorUid)
    {
        var variableSystem = collection.Resolve<VariableManager>();
        variableSystem.Set(Name, Value.GetValueObject());
    }
}

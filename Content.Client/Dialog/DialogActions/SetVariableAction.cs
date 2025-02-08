using Content.Client.Dialog.Data;
using Content.Client.GameVariables;
using Content.Client.PasterString.Data;

namespace Content.Client.Dialog.DialogActions;

public sealed partial class SetVariableAction : IDialogAction
{
    [DataField] public string Name;
    [DataField] public string Value;
    
    public void Act(IDependencyCollection collection)
    {
        var variableSystem = collection.Resolve<VariableManager>();;
        variableSystem.SetValue(Name, Value);
    }
}

[ImplicitDataDefinitionForInheritors]
public abstract partial class BaseCompareAction : IDialogAction
{
    [DataField] public SmartString First;
    [DataField] public SmartString Second;

    [DataField] public IDialogAction? If;
    [DataField] public IDialogAction? Else;

    private VariableManager _variableSystem = default!;

    public void Act(IDependencyCollection collection)
    {
        _variableSystem = collection.Resolve<VariableManager>();;
        
        if(Checkup(First, Second))
            If?.Act(collection);
        else 
            Else?.Act(collection);
    }

    protected abstract bool Checkup(string value1, string value2); 
}


public sealed partial class IsEqualsAction : BaseCompareAction
{
    protected override bool Checkup(string value1, string value2)
    {
        if (!float.TryParse(value1, out var f1) || !float.TryParse(value2, out var f2))
            return value1.Equals(value1);

        return f1 == f2;
    }
}

public sealed partial class IsGreaterAction : BaseCompareAction
{
    protected override bool Checkup(string value1, string value2)
    {
        if (!float.TryParse(value1, out var f1) || !float.TryParse(value2, out var f2))
            return false;

        return f1 > f2;
    }
}

public sealed partial class IsLessAction : BaseCompareAction
{
    protected override bool Checkup(string value1, string value2)
    {
        if (!float.TryParse(value1, out var f1) || !float.TryParse(value2, out var f2))
            return false;

        return f1 < f2;
    }
}

public sealed partial class AppendValueAction : IDialogAction
{
    [DataField] public string Name;
    [DataField] public SmartString Count = "1";
    
    public void Act(IDependencyCollection collection)
    {
        var variableSystem = collection.Resolve<VariableManager>();
        if (!float.TryParse(variableSystem.GetValue(Name, 0f.ToString()), out var value) ||
            !float.TryParse(Count, out var floatCount)) 
            throw new Exception();

        variableSystem.SetValue(Name, (value + floatCount).ToString());
    }
}
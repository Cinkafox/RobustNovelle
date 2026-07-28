namespace Content.Client.GameVariables;

public sealed partial class VariableManager
{
    [Dependency] private IViewVariablesManager _variablesManager = default!;

    private ViewVariablesContainer _container = default!;
    private VariableStringParser _variableStringParser = default!;

    public void Initialize()
    {
        _container = new ViewVariablesContainer(_variablesManager);
        _variableStringParser = new VariableStringParser(_container);
    }

    public string Parse(string text)
    {
        return _variableStringParser.Parse(text);
    }

    public object? ParseAsObject(string text)
    {
        return _variableStringParser.EvaluateToObject(text);
    }

    public void Set(string name, object? value)
    {
        _container.Set(name, value);
    }
}

public sealed class ViewVariablesContainer : IVariableContainer
{
    private readonly IViewVariablesManager _variablesManager;
    private Dictionary<string, object> _variables = new();

    public ViewVariablesContainer(IViewVariablesManager variablesManager)
    {
        _variablesManager = variablesManager;
    }
    
    public object? Get(string name)
    {
        return name.StartsWith('/') ? _variablesManager.ReadPath(name) : _variables.GetValueOrDefault(name);
    }

    public void Set(string name, object? value)
    {
        if (name.StartsWith('/'))
        {
            _variablesManager.ResolvePath(name)?.Set(value);
            return;
        }
        
        if(value != null)
            _variables[name] = value;
        else
            _variables.Remove(name);
    }

    public bool Contains(string name)
    {
        if (name.StartsWith('/'))
        {
            return _variablesManager.ResolvePath(name) != null;
        }
        
        return _variables.ContainsKey(name);
    }
}
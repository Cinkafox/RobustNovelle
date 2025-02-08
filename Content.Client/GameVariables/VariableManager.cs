namespace Content.Client.GameVariables;

public sealed class VariableManager
{
    private readonly Dictionary<string, string> _variables = new();
    public void SetValue(string name, string value)
    {
        _variables[name] = value;
    }

    public string GetValue(string name, string defa = "")
    {
        return _variables.GetValueOrDefault(name, defa);
    }

    public void Clear()
    {
        _variables.Clear();
    }
}
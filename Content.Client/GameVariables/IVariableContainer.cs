namespace Content.Client.GameVariables;

public interface IVariableContainer
{
    object? Get(string name);
    void Set(string name, object? value);
    bool Contains(string name);
}
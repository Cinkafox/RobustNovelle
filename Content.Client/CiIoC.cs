using Content.Client.GameVariables;

namespace Content.Client;

public static class CiIoC
{
    public static void Register()
    {
        IoCManager.Register<VariableManager>();
    }
}
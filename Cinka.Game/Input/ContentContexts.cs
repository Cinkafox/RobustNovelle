using Robust.Shared.Input;

namespace Cinka.Game.Input;

public sealed class ContentContexts
{
    public static void SetupContexts(IInputContextContainer contexts)
    {
        var human = contexts.GetContext("human");
        human.AddFunction(EngineKeyFunctions.MoveUp);
        human.AddFunction(EngineKeyFunctions.MoveDown);
        human.AddFunction(EngineKeyFunctions.MoveLeft);
        human.AddFunction(EngineKeyFunctions.MoveRight);
        human.AddFunction(EngineKeyFunctions.Walk);
    }
}
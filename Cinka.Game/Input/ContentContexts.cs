using Robust.Shared.Input;

namespace Cinka.Game.Input;

public sealed class ContentContexts
{
    public static void SetupContexts(IInputContextContainer contexts)
    {
        var viewer = contexts.GetContext("human");
        viewer.AddFunction(EngineKeyFunctions.GuiTabNavigateNext);
        viewer.AddFunction(EngineKeyFunctions.GuiTabNavigatePrev);
        viewer.AddFunction(EngineKeyFunctions.EscapeMenu);
        viewer.AddFunction(ContentKeyFunctions.SkipDialog);
        
    }
}
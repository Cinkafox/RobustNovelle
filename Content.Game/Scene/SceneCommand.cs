using Content.Game.Scene.Data;
using Content.Game.Scene.Manager;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Game.Scene;

public sealed class SceneCommand : IConsoleCommand
{
    public string Command => "setscene";
    public string Description => "set current scene";
    public string Help => "setscene <scenePrototype>";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var sceneManager = IoCManager.Resolve<ISceneManager>();
        if (args.Length == 0)
        {
            shell.WriteError("Need one argument");
            return;
        }

        sceneManager.LoadScene(args[0]);
    }

    public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
            return CompletionResult.FromHintOptions(
                CompletionHelper.PrototypeIDs<ScenePrototype>(), "<scenePrototype>");

        return CompletionResult.Empty;
    }
}
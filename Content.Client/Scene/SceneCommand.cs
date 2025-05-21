using Content.Client.Scene.Data;
using Content.Client.Scene.Systems;
using Robust.Shared.Console;

namespace Content.Client.Scene;

public sealed class SceneCommand : IConsoleCommand
{
    public string Command => "setscene";
    public string Description => "set current scene";
    public string Help => "setscene <scenePrototype>";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var sceneManager = IoCManager.Resolve<IEntityManager>().System<SceneSystem>();
        if (args.Length == 0)
        {
            shell.WriteError("Need one argument");
            return;
        }

        sceneManager.LoadScene(shell.Player!.AttachedEntity!.Value, args[0]);
    }

    public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
            return CompletionResult.FromHintOptions(
                CompletionHelper.PrototypeIDs<ScenePrototype>(), "<scenePrototype>");

        return CompletionResult.Empty;
    }
}
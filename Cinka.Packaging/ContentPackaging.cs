using Robust.Packaging;
using Robust.Packaging.AssetProcessing;

namespace Cinka.Packaging;

public static class ContentPackaging
{
    public static async Task WriteResources(
        string contentDir,
        AssetPass pass,
        IPackageLogger logger,
        CancellationToken cancel)
    {
        var graph = new RobustClientAssetGraph();
        pass.Dependencies.Add(new AssetPassDependency(graph.Output.Name));

        AssetGraph.CalculateGraph(graph.AllPasses.Append(pass).ToArray(), logger);

        var inputPass = graph.Input;

        await RobustClientPackaging.WriteContentAssemblies(
            inputPass,
            contentDir,
            "Cinka.Game",
            new[] { "Cinka.Game" },
            cancel);

        await RobustClientPackaging.WriteClientResources(contentDir, inputPass, cancel);

        inputPass.InjectFinished();
    }
}
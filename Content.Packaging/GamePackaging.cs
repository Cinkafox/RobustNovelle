using System.Diagnostics;
using System.IO.Compression;
using Robust.Packaging;
using Robust.Packaging.AssetProcessing;
using Robust.Packaging.AssetProcessing.Passes;
using Robust.Packaging.Utility;
using Robust.Shared.Timing;

namespace Content.Packaging;

public sealed class GamePackaging
{
    /// <summary>
    /// Be advised this can be called from server packaging during a HybridACZ build.
    /// </summary>
    public static async Task PackageClient(bool skipBuild, IPackageLogger logger)
    {
        logger.Info("Building game...");

        if (!skipBuild)
        {
            await ProcessHelpers.RunCheck(new ProcessStartInfo
            {
                FileName = "dotnet",
                ArgumentList =
                {
                    "build",
                    Path.Combine("Content.Client", "Content.Client.csproj"),
                    "-c", "Release",
                    "--nologo",
                    "/v:m",
                    "/t:Rebuild",
                    "/p:FullRelease=true",
                    "/m"
                }
            });
        }

        logger.Info("Packaging game...");

        var sw = RStopwatch.StartNew();
        {
            await using var zipFile =
                File.Open(Path.Combine("release", "Content.Client.zip"), FileMode.Create, FileAccess.ReadWrite);
            using var zip = new ZipArchive(zipFile, ZipArchiveMode.Update);
            var writer = new AssetPassZipWriter(zip);

            await WriteResources("", writer, logger, default);
            await writer.FinishedTask;
        }

        logger.Info($"Finished packaging client in {sw.Elapsed}");
    }

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

        await RobustSharedPackaging.WriteContentAssemblies(
            inputPass,
            contentDir,
            "Content.Client",
            new[] { "Content.Client", "Content.StyleSheetify.Client", "Content.StyleSheetify.Shared" },
            cancel: cancel);

        await RobustClientPackaging.WriteClientResources(contentDir, pass, cancel);

        inputPass.InjectFinished();
    }
}
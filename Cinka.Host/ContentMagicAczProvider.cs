using System.Threading;
using System.Threading.Tasks;
using Cinka.Packaging;
using Robust.Packaging;
using Robust.Packaging.AssetProcessing;
using Robust.Server.ServerStatus;
using Robust.Shared.IoC;

namespace Cinka.Host;

public sealed class ContentMagicAczProvider : IMagicAczProvider
{
    private readonly IDependencyCollection _deps;

    public ContentMagicAczProvider(IDependencyCollection deps)
    {
        _deps = deps;
    }

    public async Task Package(AssetPass pass, IPackageLogger logger, CancellationToken cancel)
    {
        var contentDir = DefaultMagicAczProvider.FindContentRootPath(_deps);
        logger.Debug(contentDir);
        await ContentPackaging.WriteResources(contentDir, pass, logger, cancel);
    }
}
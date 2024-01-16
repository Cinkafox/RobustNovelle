using System.Threading;
using System.Threading.Tasks;
using Cinka.Game.Resources;
using JetBrains.Annotations;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Cinka.Game.Parallax.Data;

[UsedImplicitly]
[DataDefinition]
public sealed partial class ImageParallaxTextureSource : IParallaxTextureSource
{
    /// <summary>
    /// Texture path.
    /// </summary>
    [DataField("path", required: true)]
    public ResPath Path { get; private set; } = default!;

    Task<Texture> IParallaxTextureSource.GenerateTexture(CancellationToken cancel)
    {
        return Task.FromResult(IoCManager.Resolve<IResourceCache>().GetTexture(Path));
    }
}


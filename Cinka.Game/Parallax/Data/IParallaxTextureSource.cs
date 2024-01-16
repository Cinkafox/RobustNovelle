using System.Threading;
using System.Threading.Tasks;
using Robust.Client.Graphics;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Cinka.Game.Parallax.Data
{
    [ImplicitDataDefinitionForInheritors]
    public partial interface IParallaxTextureSource
    {
        /// <summary>
        /// Generates or loads the texture.
        /// Note that this should be cached, but not necessarily *here*.
        /// </summary>
        Task<Texture> GenerateTexture(CancellationToken cancel = default);
    }
}


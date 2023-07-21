using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;

namespace Cinka.Game.StyleSheet;

public sealed class StyleNovelle : StyleBase
{
    public StyleNovelle(IResourceCache resCache) : base(resCache)
    {
    }

    public override Stylesheet Stylesheet { get; }
}
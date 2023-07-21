using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;

namespace Cinka.Game.StyleSheet
{
    public sealed class StylesheetManager : IStylesheetManager
    {
        [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
        [Dependency] private readonly IResourceCache _resourceCache = default!;

        public Stylesheet SheetNovelle { get; private set; } = default!;

        public void Initialize()
        {
            SheetNovelle = new StyleNovelle(_resourceCache).Stylesheet;

            _userInterfaceManager.Stylesheet = SheetNovelle;
        }
    }
}
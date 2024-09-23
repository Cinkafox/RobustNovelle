using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;

namespace Content.Game.StyleSheet;

public sealed class StylesheetManager : IStylesheetManager
{
    [Dependency] private readonly IResourceCache _resourceCache = default!;
    [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;

    public Stylesheet SheetNovelle { get; private set; } = default!;

    public void Initialize()
    {
        SheetNovelle = new StyleNovelle(_resourceCache).Stylesheet;

        _userInterfaceManager.Stylesheet = SheetNovelle;
    }
}
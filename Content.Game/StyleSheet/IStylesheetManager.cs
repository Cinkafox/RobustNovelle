using Robust.Client.UserInterface;

namespace Content.Game.StyleSheet;

public interface IStylesheetManager
{
    Stylesheet SheetNovelle { get; }

    void Initialize();
}
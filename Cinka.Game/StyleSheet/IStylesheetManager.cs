using Robust.Client.UserInterface;

namespace Cinka.Game.StyleSheet;

public interface IStylesheetManager
{
    Stylesheet SheetNovelle { get; }

    void Initialize();
}
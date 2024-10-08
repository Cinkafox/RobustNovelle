using Robust.Client;
using Robust.Shared.Utility;

namespace Content.Game;

internal static class Program
{
    public static void Main(string[] args)
    {
        ContentStart.StartLibrary(args, new GameControllerOptions
        {
            Sandboxing = true,

            ContentModulePrefix = "Content.",

            ContentBuildDirectory = "Content.Game",

            DefaultWindowTitle = "Meow",

            UserDataDirectoryName = "Cinka",

            ConfigFileName = "config.toml",

            SplashLogo = new ResPath("/Textures/Interface/loading.png"),

            // Check "RobustToolbox/Resources/Textures/Logo/icon" for an example window icon set.
            WindowIconSet = new ResPath("/Textures/Interface/meo.png"),

            // There are a few more options, be sure to check them all!
        });
    }
}
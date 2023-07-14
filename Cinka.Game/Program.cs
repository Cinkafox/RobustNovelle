using Robust.Client;

namespace Cinka.Game;

internal static class Program
{
    public static void Main(string[] args)
    {
        ContentStart.StartLibrary(args, new GameControllerOptions()
        {
            Sandboxing = false,
                
            ContentModulePrefix = "Cinka.",
                
            ContentBuildDirectory = "Cinka.Game",
                
            DefaultWindowTitle = "Meow",
                
            UserDataDirectoryName = "Cinka",
                
            ConfigFileName = "config.toml",
                
            //SplashLogo = new ResourcePath("/path/to/splash/logo.png"),
                
            // Check "RobustToolbox/Resources/Textures/Logo/icon" for an example window icon set.
            //WindowIconSet = new ResourcePath("/path/to/folder/with/window/icon/set"),
                
            // There are a few more options, be sure to check them all!
        });
    }
}
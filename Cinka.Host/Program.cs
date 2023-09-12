using Robust.Server;

namespace Cinka.Host;

internal static class Program
{
    public static void Main(string[] args)
    {
        ContentStart.StartLibrary(args, new ServerOptions
        {
            ContentModulePrefix = "Cinka.",
            ContentBuildDirectory = "Cinka.Host"
        });
    }
}
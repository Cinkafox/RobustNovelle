using Robust.Server;

namespace Content.Host;

internal static class Program
{
    public static void Main(string[] args)
    {
        ContentStart.StartLibrary(args, new ServerOptions
        {
            ContentModulePrefix = "Content.",
            ContentBuildDirectory = "Content.Host"
        });
    }
}
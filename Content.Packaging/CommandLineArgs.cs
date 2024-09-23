using System.Diagnostics.CodeAnalysis;

namespace Content.Packaging;
public sealed class CommandLineArgs
{
    // PJB forgib me

    /// <summary>
    /// Generate client or server.
    /// </summary>

    /// <summary>
    /// Should we also build the relevant project.
    /// </summary>
    public bool SkipBuild { get; set; }

    /// <summary>
    /// Should we wipe the release folder or ignore it.
    /// </summary>
    public bool WipeRelease { get; set; }

    /// <summary>
    /// Platforms for server packaging.
    /// </summary>
    public List<string>? Platforms { get; set; }

    // CommandLineArgs, 3rd of her name.
    public static bool TryParse(IReadOnlyList<string> args, [NotNullWhen(true)] out CommandLineArgs? parsed)
    {
        parsed = null;
        var skipBuild = false;
        var wipeRelease = true;
        List<string>? platforms = null;

        using var enumerator = args.GetEnumerator();
        var i = -1;

        while (enumerator.MoveNext())
        {
            i++;
            var arg = enumerator.Current;

            if (arg == "--skip-build")
            {
                skipBuild = true;
            }
            else if (arg == "--no-wipe-release")
            {
                wipeRelease = false;
            }
            else if (arg == "--platform")
            {
                if (!enumerator.MoveNext())
                {
                    Console.WriteLine("No platform provided");
                    return false;
                }

                platforms ??= new List<string>();
                platforms.Add(enumerator.Current);
            }
            else if (arg == "--help")
            {
                PrintHelp();
                return false;
            }
            else
            {
                Console.WriteLine("Unknown argument: {0}", arg);
            }
        }
        

        parsed = new CommandLineArgs(skipBuild, wipeRelease, platforms);
        return true;
    }

    private static void PrintHelp()
    {
        Console.WriteLine(@"
Usage: Content.Packaging [options]

Options:
  --skip-build          Should we skip building the project and use what's already there.
  --no-wipe-release     Don't wipe the release folder before creating files.
  --platform            Platform for server builds. Default will output several x64 targets.
");
    }

    private CommandLineArgs(
        bool skipBuild,
        bool wipeRelease,
        List<string>? platforms)
    {
        SkipBuild = skipBuild;
        WipeRelease = wipeRelease;
        Platforms = platforms;
    }
}
using System.Reflection;
using Cinka.Game;
using Robust.Shared.Analyzers;
using Robust.UnitTesting;

namespace Cinka.Test;

[Virtual]
public class CinkaUnitTest : RobustUnitTest
{
    protected override void OverrideIoC()
    {
        base.OverrideIoC();

        CiIoC.Register();
    }

    protected override Assembly[] GetContentAssemblies()
    {
        var l = new List<Assembly>
        {
            typeof(Cinka.Game.EntryPoint).Assembly,
            typeof(CinkaUnitTest).Assembly
        };

        return l.ToArray();
    }
}

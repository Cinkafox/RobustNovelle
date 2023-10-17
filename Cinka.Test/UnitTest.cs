using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

namespace Cinka.Test;

[TestFixture]
public class Tests : CinkaUnitTest
{
    private IPrototypeManager _prototypeManager;
    
    [OneTimeSetUp]
    public void OnTimeSetup()
    {
        IoCManager.Resolve<ISerializationManager>().Initialize();
        _prototypeManager = IoCManager.Resolve<IPrototypeManager>();
        _prototypeManager.Initialize();
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}
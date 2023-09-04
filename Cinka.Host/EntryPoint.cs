using Robust.Server.ServerStatus;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Cinka.Host;

public sealed class EntryPoint : GameServer
{
    public override void Init()
    {
        base.Init();

        // Configure ACZ correctly.
        var aczProvider = new ContentMagicAczProvider(IoCManager.Resolve<IDependencyCollection>());
        IoCManager.Resolve<IStatusHost>().SetMagicAczProvider(aczProvider);
        
        var factory = IoCManager.Resolve<IComponentFactory>();
        var prototypes = IoCManager.Resolve<IPrototypeManager>();

        factory.DoAutoRegistrations();
        
        foreach (var ignoreName in IgnoredComponents.List)
        {
            factory.RegisterIgnore(ignoreName);
        }
        
        prototypes.RegisterIgnore("background");
        prototypes.RegisterIgnore("scene");
        prototypes.RegisterIgnore("location");
        prototypes.RegisterIgnore("scene");

        ServerContentIoC.Register();
        
        IoCManager.BuildGraph();
            
        factory.GenerateNetIds();
    }
}


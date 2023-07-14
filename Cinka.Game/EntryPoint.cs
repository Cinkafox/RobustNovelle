using System.Globalization;
using Cinka.Game.Gameplay;
using Robust.Client;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Cinka.Game;

public sealed class EntryPoint : GameClient
{
    [Dependency] private readonly IUserInterfaceManager _ui = default!;
    
    private const string Culture = "ru-RU";

    public override void PreInit()
    {
        base.PreInit();
        
        IoCManager.Resolve<ILocalizationManager>().LoadCulture(new CultureInfo(Culture));
    }

    public override void Init()
    {
        var factory = IoCManager.Resolve<IComponentFactory>();
        
        factory.DoAutoRegistrations();

        TemplateIoC.Register();

        IoCManager.BuildGraph();
        IoCManager.InjectDependencies(this);
            
        factory.GenerateNetIds();

   }

    public override void PostInit()
    {
        base.PostInit();

        _ui.MainViewport.Visible = false;
        IoCManager.Resolve<IStateManager>().RequestStateChange<GameplayStateBase>();
        
        IoCManager.Resolve<IBaseClient>().StartSinglePlayer();
        
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
            
        // DEVNOTE: You'll want to do a proper shutdown here.
    }

    public override void Update(ModUpdateLevel level, FrameEventArgs frameEventArgs)
    {
        base.Update(level, frameEventArgs);
            
        // DEVNOTE: Game update loop goes here. Usually you'll want some independent GameTicker.
    }
}
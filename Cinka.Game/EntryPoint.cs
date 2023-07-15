using System.Globalization;
using Cinka.Game.Background.Manager;
using Cinka.Game.Gameplay;
using Cinka.Game.Input;
using Robust.Client;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Cinka.Game;

public sealed class EntryPoint : GameClient
{
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    [Dependency] private readonly IBackgroundManager _background = default!;
    [Dependency] private readonly IStateManager _stateManager = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IBaseClient _client = default!;
    [Dependency] private readonly IInputManager _inputManager = default!;
    
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

        CiIoC.Register();
        
        IoCManager.BuildGraph();
        IoCManager.InjectDependencies(this);
        
        factory.GenerateNetIds();
    }

    public override void PostInit()
    {
        base.PostInit();
        
        ContentContexts.SetupContexts(_inputManager.Contexts);

        _background.LoadBackground("default");
        _stateManager.RequestStateChange<GameplayStateBase>();

        _uiManager.MainViewport.Visible = false;
        _client.StartSinglePlayer();
        
        IoCManager.Resolve<IMapManager>().CreateMap(new MapId(1));
        var entity = _entityManager.SpawnEntity("Camera", new MapCoordinates(0, 0,new MapId(1) ));
        IoCManager.Resolve<IPlayerManager>().LocalPlayer.AttachEntity(entity,_entityManager,_client);
        IoCManager.Resolve<ILightManager>().Enabled = false;
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
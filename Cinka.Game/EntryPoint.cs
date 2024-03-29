using System.Globalization;
using Cinka.Game.Audio.Data;
using Cinka.Game.Camera.Manager;
using Cinka.Game.Gameplay;
using Cinka.Game.Input;
using Cinka.Game.Location.Managers;
using Cinka.Game.Menu;
using Cinka.Game.Scene.Manager;
using Cinka.Game.StyleSheet;
using Robust.Client;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Network.Messages;
using Robust.Shared.Prototypes;

namespace Cinka.Game;

public sealed class EntryPoint : GameClient
{
    private const string Culture = "ru-RU";
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;
    [Dependency] private readonly ICameraManager _cameraManager = default!;
    [Dependency] private readonly IBaseClient _client = default!;
    [Dependency] private readonly IComponentFactory _componentFactory = default!;
    [Dependency] private readonly IInputManager _inputManager = default!;
    [Dependency] private readonly ILocationManager _locationManager = default!;
    [Dependency] private readonly ISceneManager _sceneManager = default!;
    [Dependency] private readonly IStateManager _stateManager = default!;
    [Dependency] private readonly IStylesheetManager _stylesheetManager = default!;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    [Dependency] private readonly IResourceCache _resource = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void PreInit()
    {
        IoCManager.Resolve<ILocalizationManager>().LoadCulture(new CultureInfo(Culture));
        CiIoC.Register();
        IoCManager.BuildGraph();
        IoCManager.InjectDependencies(this);
    }

    public override void Init()
    {
        _componentFactory.DoAutoRegistrations();
        _componentFactory.GenerateNetIds();
    }

    public override void PostInit()
    {
        ContentContexts.SetupContexts(_inputManager.Contexts);

        //Нахуя нам свет в новелле да?
        IoCManager.Resolve<ILightManager>().Enabled = false;
        
        _uiManager.MainViewport.Visible = false;
        _client.StartSinglePlayer();
        _stylesheetManager.Initialize();

        _stateManager.RequestStateChange<MenuState>();
        
        //Some cache shit
        foreach (var audio in _prototype.EnumeratePrototypes<AudioPrototype>())
        {
            _resource.TryGetResource<AudioResource>(audio.Audio.GetSound(), out _);
        }
        Logger.Debug("Cached some audio shit!");
    }

    public override void Shutdown()
    {
        
    }
}
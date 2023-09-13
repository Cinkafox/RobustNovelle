using System.Globalization;
using Cinka.Game.Audio.Managers;
using Cinka.Game.Camera.Manager;
using Cinka.Game.Character.Managers;
using Cinka.Game.Gameplay;
using Cinka.Game.Input;
using Cinka.Game.Location.Managers;
using Cinka.Game.Scene.Manager;
using Cinka.Game.StyleSheet;
using Robust.Client;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Cinka.Game;

public sealed class EntryPoint : GameClient
{
    private const string Culture = "ru-RU";
    [Dependency] private readonly IAudioManager _audioManager = default!;
    [Dependency] private readonly ICameraManager _cameraManager = default!;
    [Dependency] private readonly ICharacterManager _characterManager = default!;
    [Dependency] private readonly IBaseClient _client = default!;
    [Dependency] private readonly IComponentFactory _componentFactory = default!;
    [Dependency] private readonly IInputManager _inputManager = default!;
    [Dependency] private readonly ILocationManager _locationManager = default!;
    [Dependency] private readonly ISceneManager _sceneManager = default!;
    [Dependency] private readonly IStateManager _stateManager = default!;
    [Dependency] private readonly IStylesheetManager _stylesheetManager = default!;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;

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

        _stateManager.RequestStateChange<GameplayStateBase>();
        _uiManager.MainViewport.Visible = false;
        _client.StartSinglePlayer();

        _audioManager.Initialize();
        _characterManager.Initialize();
        _locationManager.Initialize();
        _cameraManager.Initialize();
        _stylesheetManager.Initialize();
        _sceneManager.Initialize();
    }

    public override void Shutdown()
    {
        _audioManager.Shutdown();
    }
}
using System.Globalization;
using Content.Game.Audio.Data;
using Content.Game.Input;
using Content.Game.Menu;
using Robust.Client;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Audio;
using Robust.Shared.ContentPack;
using Robust.Shared.Prototypes;
using Content.StyleSheetify.Client.StyleSheet;

namespace Content.Game;

public sealed class EntryPoint : GameClient
{
    private const string Culture = "ru-RU";
    
    [Dependency] private readonly IBaseClient _client = default!;
    [Dependency] private readonly IComponentFactory _componentFactory = default!;
    [Dependency] private readonly IInputManager _inputManager = default!;
    [Dependency] private readonly IStateManager _stateManager = default!;
    [Dependency] private readonly IStyleSheetManager _styleSheetManager = default!;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    [Dependency] private readonly IResourceCache _resource = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IClyde _clyde = default!;

    public override void PreInit()
    {
        IoCManager.Resolve<ILocalizationManager>().LoadCulture(new CultureInfo(Culture));
        CiIoC.Register(); ;
        StyleSheetify.Client.DependencyRegistration.Register(IoCManager.Instance!);
        IoCManager.BuildGraph();
        IoCManager.InjectDependencies(this);
        _clyde.SetWindowTitle("LOADING: [------]");
    }

    public override void Init()
    {
        _clyde.SetWindowTitle("LOADING: [#-----]");
        _componentFactory.DoAutoRegistrations();
        _clyde.SetWindowTitle("LOADING: [##----]");
        _componentFactory.GenerateNetIds();
    }

    public override void PostInit()
    {
        ContentContexts.SetupContexts(_inputManager.Contexts);
        _clyde.SetWindowTitle("LOADING: [###---]");
        //Нахуя нам свет в новелле да?
        IoCManager.Resolve<ILightManager>().Enabled = false;
        
        _uiManager.MainViewport.Visible = false;
        _client.StartSinglePlayer();
        _uiManager.SetDefaultTheme("DefaultTheme");
        _styleSheetManager.ApplyStyleSheet("default");

        _stateManager.RequestStateChange<MenuState>();
        
        _clyde.SetWindowTitle("LOADING: [####--]");
        //Some cache shit
        foreach (var audio in _prototype.EnumeratePrototypes<AudioPrototype>())
        {
            var specifier = audio.Audio;
            if (specifier is SoundPathSpecifier pathSpecifier)
            {
                _resource.TryGetResource<AudioResource>(pathSpecifier.Path, out _);
            }

            if (specifier is SoundCollectionSpecifier collectionSpecifier
                && _prototype.TryIndex<SoundCollectionPrototype>(collectionSpecifier.Collection!, out var collectionPrototype))
            {
                foreach (var resPath in collectionPrototype.PickFiles)
                {
                    _resource.TryGetResource<AudioResource>(resPath, out _);
                }
            }
        }
        _clyde.SetWindowTitle("LOADING: [#####-]");
        Logger.Debug("Cached some audio shit!");
        
        _clyde.SetWindowTitle("Femboy adventure");
    }
}
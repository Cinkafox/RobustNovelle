using System.Globalization;
using System.Timers;
using Content.Client.Gameplay;
using Content.Client.GameVariables;
using Content.Client.Input;
using Content.Client.Menu;
using Content.Client.Tile;
using Content.StyleSheetify.Client.StyleSheet;
using Robust.Client;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Timer = Robust.Shared.Timing.Timer;

namespace Content.Client.Entry;

public sealed partial class EntryPoint : GameClient
{
    private const string Culture = "ru-RU";
    
    [Dependency] private IClyde _clyde = default!;
    [Dependency] private IComponentFactory _componentFactory = default!;
    [Dependency] private IConfigurationManager _configurationManager = default!;
    [Dependency] private IInputManager _inputManager = default!;
    [Dependency] private IPrototypeManager _prototype = default!;
    [Dependency] private IResourceCache _resource = default!;
    [Dependency] private IStateManager _stateManager = default!;
    [Dependency] private ITileDefinitionManager _tileDefinitionManager = default!;
    [Dependency] private IUserInterfaceManager _uiManager = default!;
    [Dependency] private IBaseClient _baseClient = default!;
    [Dependency] private IGameController _gameController = default!;
    [Dependency] private VariableManager _variableManager = default!;

    public override void PreInit()
    {
        IoCManager.Resolve<ILocalizationManager>().LoadCulture(new CultureInfo(Culture));
        CiIoC.Register();
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
        InitTileDefinitions();
        ContentContexts.SetupContexts(_inputManager.Contexts);
        _clyde.SetWindowTitle("LOADING: [###---]");
        //Нахуя нам свет в новелле да?
        IoCManager.Resolve<ILightManager>().Enabled = false;

        _uiManager.MainViewport.Visible = false;
        _uiManager.SetDefaultTheme("DefaultTheme");
        IoCManager.Resolve<IContentStyleSheetManager>().ApplyStyleSheet("default");

        _clyde.SetWindowTitle("LOADING: [####--]");

        //Some cache TODO: Find out how to cache nonCollection audio
        foreach (var soundCollection in _prototype.EnumeratePrototypes<SoundCollectionPrototype>())
        foreach (var resPath in soundCollection.PickFiles)
            _resource.TryGetResource<AudioResource>(resPath, out _);
        
        _variableManager.Initialize();

        _clyde.SetWindowTitle("LOADING: [#####-]");

        _baseClient.RunLevelChanged += BaseClientOnRunLevelChanged;
        
        
        if (_configurationManager.GetCVar(CCVars.CCVars.GameLoadImmediately))
            _baseClient.StartSinglePlayer();
        else
            _stateManager.RequestStateChange<MenuState>();
    }

    private void BaseClientOnRunLevelChanged(object? sender, RunLevelChangedEventArgs e)
    {
        if (e.NewLevel.IsInGameLike())
        {
            Timer.Spawn(0, () =>
            {
                _clyde.SetWindowTitle("Game");
                _stateManager.RequestStateChange<GameplayState>();
            });
        }
        else
        {
            if (_configurationManager.GetCVar(CCVars.CCVars.GameLoadImmediately))
            {
                _gameController.Shutdown();
                return;
            }
            
            _clyde.SetWindowTitle("Main menu...");
            _stateManager.RequestStateChange<MenuState>();
        }
    }


    private void InitTileDefinitions()
    {
        var prototypeList = new List<ContentTileDefinition>();

        _tileDefinitionManager.Register(new VoidTile());

        foreach (var tileDef in _prototype.EnumeratePrototypes<ContentTileDefinition>())
        {
            tileDef.Sprite = tileDef.SpriteSpecifier switch
            {
                SpriteSpecifier.Texture texture => texture.TexturePath,
                SpriteSpecifier.Rsi rsi => rsi.RsiPath / $"{rsi.RsiState}.png",
                _ => tileDef.Sprite
            };

            if (string.IsNullOrEmpty(tileDef.Name))
                tileDef.Name = Loc.GetString(tileDef.ID);

            prototypeList.Add(tileDef);
        }

        prototypeList.Sort((a, b) => string.Compare(a.ID, b.ID, StringComparison.Ordinal));

        foreach (var tileDef in prototypeList) _tileDefinitionManager.Register(tileDef);

        _tileDefinitionManager.Initialize();
    }
}

[Prototype("VoidTile")]
public sealed partial class VoidTile : ITileDefinition, IPrototype
{
    public ushort TileId { get; set; }
    public string Name { get; } = "Void";
    [IdDataField] public string ID { get; set; } = "Void";
    public ResPath? Sprite { get; }
    public Dictionary<Direction, ResPath> EdgeSprites { get; } = new();
    public int EdgeSpritePriority { get; }
    public float Friction { get; }
    public byte Variants { get; }

    public void AssignTileId(ushort id)
    {
        TileId = id;
    }
}
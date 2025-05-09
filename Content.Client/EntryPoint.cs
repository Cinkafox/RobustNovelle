using System.Globalization;
using Content.Client.Audio.Data;
using Content.Client.Input;
using Content.Client.Menu;
using Content.Client.Tile;
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
using Robust.Shared.Map;
using Robust.Shared.Utility;

namespace Content.Client;

public sealed class EntryPoint : GameClient
{
    private const string Culture = "ru-RU";
    
    [Dependency] private readonly IBaseClient _client = default!;
    [Dependency] private readonly IComponentFactory _componentFactory = default!;
    [Dependency] private readonly ITileDefinitionManager _tileDefinitionManager = default!;
    [Dependency] private readonly IInputManager _inputManager = default!;
    [Dependency] private readonly IStateManager _stateManager = default!;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    [Dependency] private readonly IResourceCache _resource = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IClyde _clyde = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

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
        
        _clyde.SetWindowTitle("Main menu...");
    }
    
    
    private void InitTileDefinitions()
    {
        var prototypeList = new List<ContentTileDefinition>();
        
        _tileDefinitionManager.Register(new VoidTile());
        
        foreach (var tileDef in _prototypeManager.EnumeratePrototypes<ContentTileDefinition>())
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

        foreach (var tileDef in prototypeList)
        {
            _tileDefinitionManager.Register(tileDef);
        }

        _tileDefinitionManager.Initialize();
    }
}

[Prototype("VoidTile")]
public sealed class VoidTile : ITileDefinition, IPrototype
{
    public ushort TileId { get; set; }
    public string Name { get; } = "Void";
    [IdDataField] public string ID { get; } = "Void";
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


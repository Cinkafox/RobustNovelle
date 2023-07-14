using Cinka.Game.Background.Data;
using JetBrains.Annotations;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.Utility;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;

namespace Cinka.Game.Background.Manager;

public sealed class BackgroundManager : IBackgroundManager
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    private Texture[] _currentBackground;
    private BackgoundPrototype _currentBackgroundPrototype;

    public BackgroundManager()
    {
        
    }
    
    public Texture[] GetCurrentBackground()
    {
        return _currentBackground;
    }

    public void LoadBackground(string name)
    {
        _currentBackgroundPrototype = _prototypeManager.Index<BackgoundPrototype>(name);
        _currentBackground = new Texture[_currentBackgroundPrototype.Layers.Count];
        

        for (var i = 0; i < _currentBackground.Length; i++)
        {
            var layer = _currentBackgroundPrototype.Layers[i];
            if(!TryGetRSI(layer,out var rsi) || !rsi.TryGetState(layer.State,out var state))
                continue;

            _currentBackground[i] = state.Frame0;
        }
    }

    private bool TryGetRSI(PrototypeLayerData data, [CanBeNull] out RSI rsi)
    {
        rsi = null;

        if (data.RsiPath != null)
            rsi = StaticIoC.ResC.GetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / data.RsiPath).RSI;
        else if (_currentBackgroundPrototype.RsiPath != null)
            rsi = StaticIoC.ResC
                .GetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / _currentBackgroundPrototype.RsiPath)
                .RSI;

        return rsi != null;
    }

    public void UnloadBackground()
    {
        _currentBackground = null;
    }
}
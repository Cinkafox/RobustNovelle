using System;
using System.Diagnostics.CodeAnalysis;
using Cinka.Game.Background.Data;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Cinka.Game.Background.Manager;

//TODO: БЛЯТЬ ПЕРЕТАЩИТЬ ЭТУ ПОЕБОТУ В СИСТЕМУ! Чтобы энтити хуентити все дела, и чтобы бэки были в виде энтити
public sealed class BackgroundManager : IBackgroundManager
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IResourceCache _cache = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    private Texture[] _currentBackground = [];
    private Texture[] _fadingBackground = [];
    private BackgroundPrototype? _currentBackgroundPrototype;
    private TimeSpan _lastFadingBackgroundUpdateCurTime;
    private string _currentName = String.Empty;

    public Texture[] GetCurrentBackground()
    {
        return _currentBackground;
    }

    public Texture[] GetFadingBackground()
    {
        return _fadingBackground;
    }

    public void ClearFadingBackground()
    {
        _fadingBackground = [];
    }

    public bool TryGetFadingBackground(out Texture[] textures)
    {
        textures = _fadingBackground;
        return _fadingBackground.Length != 0;
    }

    public TimeSpan GetLastFadingBackgroundUpdateCurTime()
    {
        return _lastFadingBackgroundUpdateCurTime;
    }

    public void LoadBackground(string name)
    {
        if(name == _currentName) return;
        _currentName = name;
        
        Logger.Debug("LOADED BACKGROUND " + name);
        _fadingBackground = _currentBackground;
        _lastFadingBackgroundUpdateCurTime = _gameTiming.CurTime;
        
        _currentBackgroundPrototype = _prototypeManager.Index<BackgroundPrototype>(name);
        _currentBackground = new Texture[_currentBackgroundPrototype.Layers.Count + 1];

        var backState = _currentBackgroundPrototype.State ?? "default";
        if (!TryGetRSI(null, out var brsi) || !brsi.TryGetState(backState, out var bstate))
            return;
        _currentBackground[0] = bstate.Frame0;

        for (var i = 0; i < _currentBackgroundPrototype.Layers.Count; i++)
        {
            var layer = _currentBackgroundPrototype.Layers[i];
            var layerState = layer.State ?? "default";
            if (!TryGetRSI(layer, out var rsi) || !rsi.TryGetState(layerState, out var state))
                state = bstate;

            _currentBackground[i + 1] = state.Frame0;
        }
    }

    public void UnloadBackground()
    {
        _currentBackground = System.Array.Empty<Texture>();
        _fadingBackground = System.Array.Empty<Texture>();
    }


    private bool TryGetRSI(PrototypeLayerData? data,[NotNullWhen(true)] out RSI? rsi)
    {
        rsi = null;

        if (data?.RsiPath != null)
            rsi = _cache.GetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / data.RsiPath).RSI;
        else if (_currentBackgroundPrototype?.RsiPath != null)
            rsi = _cache
                .GetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / _currentBackgroundPrototype.RsiPath)
                .RSI;

        return rsi != null;
    }
}
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Cinka.Game.Parallax.Data;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using ParallaxLayerConfig = Cinka.Game.Parallax.Data.ParallaxLayerConfig;

namespace Cinka.Game.Parallax.Managers;

public sealed class ParallaxManager : IParallaxManager
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    private ISawmill _sawmill = Logger.GetSawmill("parallax");

    public Vector2 ParallaxAnchor { get; set; }

    private readonly Dictionary<string, ParallaxLayerPrepared[]> _parallaxes = new();

    private readonly Dictionary<string, CancellationTokenSource> _loadingParallaxes = new();

    public bool IsLoaded(string name) => _parallaxes.ContainsKey(name);

    public ParallaxLayerPrepared[] GetParallaxLayers(string name)
    {
        return !_parallaxes.TryGetValue(name, out var lq) ? Array.Empty<ParallaxLayerPrepared>() : lq;
    }

    public void UnloadParallax(string name)
    {
        if (_loadingParallaxes.TryGetValue(name, out var loading))
        {
            loading.Cancel();
            _loadingParallaxes.Remove(name, out _);
            return;
        }

        if (!_parallaxes.ContainsKey(name)) return;
        _parallaxes.Remove(name);
    }

    public async void LoadDefaultParallax()
    {
        _sawmill.Level = LogLevel.Info;
        await LoadParallaxByName("Default");
    }

    public async Task LoadParallaxByName(string name)
    {
        if (_parallaxes.ContainsKey(name) || _loadingParallaxes.ContainsKey(name)) return;

        // Cancel any existing load and setup the new cancellation token
        var token = new CancellationTokenSource();
        _loadingParallaxes[name] = token;
        var cancel = token.Token;

        // Begin (for real)
        _sawmill.Debug($"Loading parallax {name}");

        try
        {
            var parallaxPrototype = _prototypeManager.Index<ParallaxPrototype>(name);

            ParallaxLayerPrepared[][] layers = new ParallaxLayerPrepared[2][];
            layers[0] = layers[1] = await LoadParallaxLayers(parallaxPrototype.Layers, cancel);

            _loadingParallaxes.Remove(name, out _);

            if (token.Token.IsCancellationRequested) return;

            _parallaxes[name] = layers[1];

        }
        catch (Exception ex)
        {
            _sawmill.Error($"Failed to loaded parallax {name}: {ex}");
        }
    }

    private async Task<ParallaxLayerPrepared[]> LoadParallaxLayers(List<ParallaxLayerConfig> layersIn, CancellationToken cancel = default)
    {
        // Because this is async, make sure it doesn't change (prototype reloads could muck this up)
        // Since the tasks aren't awaited until the end, this should be fine
        var tasks = new Task<ParallaxLayerPrepared>[layersIn.Count];
        for (var i = 0; i < layersIn.Count; i++)
        {
            tasks[i] = LoadParallaxLayer(layersIn[i], cancel);
        }
        return await Task.WhenAll(tasks);
    }

    private async Task<ParallaxLayerPrepared> LoadParallaxLayer(ParallaxLayerConfig config, CancellationToken cancel = default)
    {
        return new ParallaxLayerPrepared()
        {
            Texture = await config.Texture.GenerateTexture(cancel),
            Config = config
        };
    }
}


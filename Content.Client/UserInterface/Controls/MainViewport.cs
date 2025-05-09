using System.Collections.Immutable;
using Content.Client.Viewport;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;

namespace Content.Client.UserInterface.Controls;

/// <summary>
///     Wrapper for <see cref="ScalingViewport" /> that listens to configuration variables.
///     Also does NN-snapping within tolerances.
/// </summary>
public sealed class MainViewport : UIWidget
{
    private static readonly HashSet<MainViewport> InstancesPrivate = [];
    public static ImmutableHashSet<MainViewport> Instances => InstancesPrivate.ToImmutableHashSet();
    public static void UpdateConfAll()
    {
        foreach (var instance in InstancesPrivate)
        {
            instance.UpdateCfg();
        }
    }
    
    [Dependency] private readonly IConfigurationManager _cfg = default!;

    public MainViewport()
    {
        IoCManager.InjectDependencies(this);

        Viewport = new ScalingViewport
        {
            AlwaysRender = true,
            RenderScaleMode = ScalingViewportRenderScaleMode.CeilInt,
            MouseFilter = MouseFilterMode.Stop
        };

        AddChild(Viewport);
        InstancesPrivate.Add(this);
    }

    ~MainViewport()
    {
        InstancesPrivate.Remove(this);
    }

    public ScalingViewport Viewport { get; }
    
    public void UpdateCfg()
    {
        var stretch = _cfg.GetCVar(CCVars.CCVars.ViewportStretch);
        var renderScaleUp = _cfg.GetCVar(CCVars.CCVars.ViewportScaleRender);
        var fixedFactor = _cfg.GetCVar(CCVars.CCVars.ViewportFixedScaleFactor);

        if (stretch)
        {
            Viewport.FixedStretchSize = null;
            Viewport.StretchMode = ScalingViewportStretchMode.Bilinear;

            if (renderScaleUp)
            {
                Viewport.RenderScaleMode = ScalingViewportRenderScaleMode.CeilInt;
            }
            else
            {
                Viewport.RenderScaleMode = ScalingViewportRenderScaleMode.Fixed;
                Viewport.FixedRenderScale = 1;
            }

            return;
        }
        
        Viewport.FixedStretchSize = Viewport.ViewportSize * fixedFactor;
        Viewport.StretchMode = ScalingViewportStretchMode.Nearest;

        if (renderScaleUp)
        {
            Viewport.RenderScaleMode = ScalingViewportRenderScaleMode.Fixed;
            Viewport.FixedRenderScale = fixedFactor;
        }
        else
        {
            // Snapping but forced to render scale at scale 1 so...
            // At least we can NN.
            Viewport.RenderScaleMode = ScalingViewportRenderScaleMode.Fixed;
            Viewport.FixedRenderScale = 1;
        }
    }
    

    protected override void Resized()
    {
        base.Resized();

        UpdateCfg();
    }
}
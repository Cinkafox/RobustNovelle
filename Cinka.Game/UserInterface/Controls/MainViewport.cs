using System.Numerics;
using Cinka.Game.Viewport;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;

namespace Cinka.Game.UserInterface.Controls;

/// <summary>
///     Wrapper for <see cref="ScalingViewport" /> that listens to configuration variables.
///     Also does NN-snapping within tolerances.
/// </summary>
public sealed class MainViewport : UIWidget
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly ViewportManager _vpManager = default!;

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
    }

    public ScalingViewport Viewport { get; }

    protected override void EnteredTree()
    {
        base.EnteredTree();

        _vpManager.AddViewport(this);
    }

    protected override void ExitedTree()
    {
        base.ExitedTree();

        _vpManager.RemoveViewport(this);
    }

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
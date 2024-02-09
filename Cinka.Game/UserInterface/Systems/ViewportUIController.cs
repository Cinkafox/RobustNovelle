using Cinka.Game.Input;
using Cinka.Game.UserInterface.Controls;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Cinka.Game.UserInterface.Systems;

public sealed class ViewportUIController : UIController
{
    public const int ViewportHeight = 9;

    public static readonly Vector2i ViewportSize = (EyeManager.PixelsPerMeter * 21, EyeManager.PixelsPerMeter * 15);
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;
    [Dependency] private readonly IEntityManager _entMan = default!;
    [Dependency] private readonly IEyeManager _eyeManager = default!;
    [Dependency] private readonly IPlayerManager _playerMan = default!;
    private MainViewport? Viewport => UIManager.ActiveScreen?.GetWidget<MainViewport>();

    public override void Initialize()
    {
        _configurationManager.OnValueChanged(CCVars.CCVars.ViewportWidth, _ => UpdateViewportRatio());

        var gameplayStateLoad = UIManager.GetUIController<GameplayStateLoadController>();
        gameplayStateLoad.OnScreenLoad += OnScreenLoad;
    }

    private void OnScreenLoad()
    {
        ReloadViewport();
    }

    private void UpdateViewportRatio()
    {
        if (Viewport == null) return;
        
        var width = _configurationManager.GetCVar(CCVars.CCVars.ViewportWidth);

        Viewport.Viewport.ViewportSize =
            (EyeManager.PixelsPerMeter * width, EyeManager.PixelsPerMeter * ViewportHeight);
    }

    public void ReloadViewport()
    {
        if (Viewport == null) return;

        UpdateViewportRatio();
        Viewport.Viewport.HorizontalExpand = true;
        Viewport.Viewport.VerticalExpand = true;
        _eyeManager.MainViewport = Viewport.Viewport;
    }

    public override void FrameUpdate(FrameEventArgs e)
    {
        if (Viewport == null) return;

        base.FrameUpdate(e);

        Viewport.Viewport.Eye = _eyeManager.CurrentEye;

        // verify that the current eye is not "null". Fuck IEyeManager.

        var ent = _playerMan.LocalEntity;
        if (_eyeManager.CurrentEye.Position != default || ent == null)
            return;
        
        if (_entMan.TryGetComponent(ent, out EyeComponent? eye) && eye.Eye == _eyeManager.CurrentEye
            && _entMan.GetComponent<TransformComponent>(ent.Value).WorldPosition == default)
            return; // nothing to worry about, the player is just in null space... actually that is probably a problem?

        // Currently, this shouldn't happen. This likely happened because the main eye was set to null. When this
        // does happen it can create hard to troubleshoot bugs, so lets print some helpful warnings:
        Logger.Warning(
            $"Main viewport's eye is in nullspace (main eye is null?). Attached entity: {_entMan.ToPrettyString(ent.Value)}. Entity has eye comp: {eye != null}");
    }
}
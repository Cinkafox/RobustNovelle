using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Utility;
using BackgroundComponent = Content.Client.Background.Components.BackgroundComponent;

namespace Content.Client.Background;

using BackgroundComponent = BackgroundComponent;

public sealed partial class BackgroundSystem : EntitySystem
{
    [Dependency] private IResourceCache _cache = default!;
    [Dependency] private IOverlayManager _overlay = default!;
    [Dependency] private TransformSystem _transform = default!;
    
    public const int BackgroundZIndex = 0;
    public const string FadeAnimationKey = "fade";
    
    public override void Initialize()
    {
        _overlay.AddOverlay(new BackgroundOverlay());

        SubscribeLocalEvent<BackgroundComponent, AnimationCompletedEvent>(OnAnimationComplete);
    }

    private void OnAnimationComplete(EntityUid uid, BackgroundComponent component, AnimationCompletedEvent args)
    {
        if (args.Key != FadeAnimationKey)
            return;

        QueueDel(uid);
    }

    public void LoadBackground(EntityUid mapUid, ResPath? path)
    {
        Log.Info($"Loading background: {path}");
        
        if (path == null)
        {
            return;
        }
        
        var backgroundComp = EnsureComp<BackgroundComponent>(mapUid);
        backgroundComp.Layer = _cache.GetResource<TextureResource>(path.Value).Texture;
    }
}
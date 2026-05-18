using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Utility;
using BackgroundComponent = Content.Client.Background.Components.BackgroundComponent;

namespace Content.Client.Background;

using BackgroundComponent = BackgroundComponent;

public sealed partial class BackgroundSystem : EntitySystem
{
    [Dependency] private AnimationPlayerSystem _animationPlayer = default!;
    [Dependency] private IResourceCache _cache = default!;
    [Dependency] private IOverlayManager _overlay = default!;
    [Dependency] private TransformSystem _transform = default!;
    
    
    public const int BackgroundZIndex = 0;
    public const string DefaultState = "default";
    public const string FadeAnimationKey = "fade";
    
    [ViewVariables] private Entity<BackgroundComponent>? _backgroundUid;
    [ViewVariables] private Entity<BackgroundComponent>? _fadingUid;
    
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

        _fadingUid = _backgroundUid;
        if (_fadingUid.HasValue)
            Fade(_fadingUid.Value);

        if (path == null)
        {
            _backgroundUid = null;
            return;
        }

        var uid = EntityManager.Spawn();
        _transform.SetParent(uid, mapUid);

        var backgroundComp = EnsureComp<BackgroundComponent>(uid);
        backgroundComp.Layer = _cache.GetResource<TextureResource>(path.Value).Texture;
        _backgroundUid = new Entity<BackgroundComponent>(uid, backgroundComp);
    }

    private void Fade(Entity<BackgroundComponent> entity, int fadeTime = 1)
    {
        var animationPlayer = EnsureComp<AnimationPlayerComponent>(entity);
        _animationPlayer.Play(new Entity<AnimationPlayerComponent>(entity, animationPlayer), new Animation
        {
            Length = TimeSpan.FromSeconds(fadeTime),
            AnimationTracks =
            {
                new AnimationTrackComponentProperty
                {
                    ComponentType = typeof(BackgroundComponent),
                    Property = nameof(BackgroundComponent.Visibility),
                    KeyFrames =
                    {
                        new AnimationTrackProperty.KeyFrame(entity.Comp.Visibility, 0),
                        new AnimationTrackProperty.KeyFrame(0, fadeTime)
                    }
                }
            }
        }, FadeAnimationKey);
    }
}
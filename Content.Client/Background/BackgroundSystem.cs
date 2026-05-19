using Content.Client.Viewport;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.State;
using Robust.Shared.Spawners;
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
    [Dependency] private IStateManager _stateManager = default!;
    [Dependency] private IClyde _clyde = default!;
    
    public const int BackgroundZIndex = 0;
    public const string FadeAnimationKey = "fade";
    
    [ViewVariables] private Entity<BackgroundComponent>? _backgroundUid;
    
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
        
        if (_backgroundUid.HasValue)
        {
            _transform.SetParent(_backgroundUid.Value, mapUid);
            Fade(_backgroundUid.Value);
        }
        else
        {
            FadeScreen(mapUid);
        }

        if (path == null)
        {
            _backgroundUid = null;
            return;
        }
        
        _backgroundUid = CreateBackground(mapUid, _cache.GetResource<TextureResource>(path.Value).Texture);
    }

    private Entity<BackgroundComponent> CreateBackground(EntityUid mapUid, Texture texture)
    {
        var uid = Spawn();
        _transform.SetParent(uid, mapUid);

        var backgroundComp = EnsureComp<BackgroundComponent>(uid);
        backgroundComp.Layer = texture;

        return new Entity<BackgroundComponent>(uid, backgroundComp);
    }

    private void FadeScreen(EntityUid mapUid, int fadeTime = 1)
    {
        if(_stateManager.CurrentState is not IMainViewportState state)
            return;
        
        state.Viewport.Viewport.ScreenshotNow(pixels =>
        {
            var texture = _clyde.LoadTextureFromImage(pixels);
            var back = CreateBackground(mapUid, texture);
            Fade(back, fadeTime);
        });
    }

    private void Fade(Entity<BackgroundComponent> entity, int fadeTime = 1)
    {
        var animationPlayer = EnsureComp<AnimationPlayerComponent>(entity);
        EnsureComp<TimedDespawnComponent>(entity).Lifetime = fadeTime;
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
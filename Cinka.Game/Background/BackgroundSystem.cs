using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cinka.Game.UserInterface.Systems;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.ViewVariables;
using BackgroundComponent = Cinka.Game.Background.Components.BackgroundComponent;

namespace Cinka.Game.Background;

public sealed class BackgroundSystem : EntitySystem
{
    public const int BackgroundZIndex = 0;
    public const string DefaultState = "default";
    public const string FadeAnimationKey = "fade";
    
    [Dependency] private readonly IOverlayManager _overlay = default!;
    [Dependency] private readonly AnimationPlayerSystem _animationPlayer = default!;
    [Dependency] private readonly IResourceCache _cache = default!;
    [Dependency] private readonly IUserInterfaceManager _userInterface = default!;

    private BackgroundUIController _backgroundController = default!;

    [ViewVariables] private Entity<BackgroundComponent>? _backgroundUid;
    [ViewVariables] private Entity<BackgroundComponent>? _fadingUid;

    public override void Initialize()
    {
        //_overlay.AddOverlay(new BackgroundOverlay());
        _backgroundController = _userInterface.GetUIController<BackgroundUIController>();
        
        SubscribeLocalEvent<BackgroundComponent,ComponentInit>(OnInitialize);
        SubscribeLocalEvent<BackgroundComponent, ComponentChange>(OnChange);
        SubscribeLocalEvent<BackgroundComponent,ComponentRemove>(OnRemove);
        SubscribeLocalEvent<BackgroundComponent,AnimationCompletedEvent>(OnAnimationComplete);
    }
    
    private void OnInitialize(EntityUid uid, BackgroundComponent component, ComponentInit args)
    {
        if (!TryGetRSI(uid, out var rsi, component))
        {
            Log.Error("Error loading background " + component.Layer.RsiPath);
            QueueDel(uid);
            return;
        }
        
        var state = component.Layer.State ?? DefaultState;

        if (!rsi.TryGetState(state, out var text))
        {
            Log.Error("Error loading background state " + component.Layer.RsiPath + " " + state);
            QueueDel(uid);
            return;
        }
        
        Log.Debug("Success loading " + component.Layer.RsiPath);
        
        component._layer = text.Frame0;
        
        _backgroundController.Ensure(new Entity<BackgroundComponent>(uid,component));
    }

    private void OnRemove(EntityUid uid, BackgroundComponent component, ComponentRemove args)
    {
        _backgroundController.Remove(uid);
    }

    private void OnChange(EntityUid uid, BackgroundComponent component, ComponentChange args)
    {
        Logger.Debug("MEOW");
    }


    private void OnAnimationComplete(EntityUid uid, BackgroundComponent component, AnimationCompletedEvent args)
    {
        if(args.Key != FadeAnimationKey)
            return;
        
        Log.Debug("End fading and deleting" + component.Layer.RsiPath);
        
        QueueDel(uid);
    }
    
    public void LoadBackground(string name)
    {
        _fadingUid = _backgroundUid;
        
        var uid = EntityManager.Spawn(name);
        _backgroundUid = new Entity<BackgroundComponent>(uid,Comp<BackgroundComponent>(uid));
        
        if(_fadingUid.HasValue)
            Fade(_fadingUid.Value);
    }
    
    private void Fade(Entity<BackgroundComponent> entity,int fadeTime = 1)
    {
        Log.Debug("starting fading " + entity.Comp.Layer.RsiPath);
        
        var animationPlayer = EnsureComp<AnimationPlayerComponent>(entity);
        _animationPlayer.Play(new Entity<AnimationPlayerComponent>(entity,animationPlayer),new Animation
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
                        new AnimationTrackProperty.KeyFrame(entity.Comp.Visibility,0),
                        new AnimationTrackProperty.KeyFrame(0,fadeTime),
                    }
                }
            }
        },FadeAnimationKey);
    }
    
    private bool TryGetRSI(EntityUid uid,[NotNullWhen(true)] out RSI? rsi,BackgroundComponent? component = null)
    {
        rsi = null;
        if (!Resolve(uid, ref component) || component.Layer.RsiPath == null || !_cache
                .TryGetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / component.Layer.RsiPath,
                    out var rsiResource)) return false;
        
        rsi = rsiResource.RSI;
        return true;
    }
}
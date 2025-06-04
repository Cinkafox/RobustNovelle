using System.Numerics;
using Content.Client.Interaction.Components;
using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.Enums;

namespace Content.Client.Interaction.Systems;

public sealed class InteractionOverlay : Overlay
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IResourceCache _resCache = default!;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
 
    private readonly Font font;
    public InteractionOverlay()
    {
        IoCManager.InjectDependencies(this);
        font = _resCache.GetFont("/Fonts/Minecraft/minecraft.ttf",25);
    }
    
    public override OverlaySpace Space => OverlaySpace.ScreenSpace;
    protected override void Draw(in OverlayDrawArgs args)
    {
        var handle = args.ScreenHandle;

        var query = _entityManager.EntityQueryEnumerator<InteractionComponent>();
        while (query.MoveNext(out var interactionComponent))
        {
            if(!interactionComponent.IsEnabled || 
               interactionComponent.CurrentInteractible is null || 
               interactionComponent.CurrentInteractible.Value.Item1.InvokeImmediately) continue;
            var text = "E: " + interactionComponent.CurrentInteractible.Value.Item1.Name;
            
            
            var size = handle.GetDimensions(font, text, 1);
            var startPos = new Vector2(args.ViewportBounds.Right / 2 - size.X / 2,20);
            
            handle.DrawRect(UIBox2.FromDimensions(startPos - new Vector2(5) - new Vector2(20,0), size + new Vector2(5 * 2) + new Vector2(20 * 2,0)) , Color.FromHex("#22222299"));
            handle.DrawRect(UIBox2.FromDimensions(startPos - new Vector2(5) - new Vector2(10,0), size + new Vector2(5 * 2) + new Vector2(10 * 2,0)) , Color.FromHex("#222222CC"));
            handle.DrawRect(UIBox2.FromDimensions(startPos - new Vector2(5), size + new Vector2(5 * 2)), Color.FromHex("#222222"));
            
            handle.DrawString(font, startPos, text);
        }
    }
}
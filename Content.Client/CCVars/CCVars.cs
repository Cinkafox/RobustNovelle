using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Client.CCVars;

[CVarDefs]
public sealed class CCVars : CVars
{
    /*
        * VIEWPORT
        */

    public static readonly CVarDef<bool> ViewportStretch =
        CVarDef.Create("viewport.stretch", true, CVar.CLIENTONLY | CVar.ARCHIVE);

    public static readonly CVarDef<int> ViewportFixedScaleFactor =
        CVarDef.Create("viewport.fixed_scale_factor", 3, CVar.CLIENTONLY | CVar.ARCHIVE);

    public static readonly CVarDef<bool> ViewportScaleRender =
        CVarDef.Create("viewport.scale_render", true, CVar.CLIENTONLY | CVar.ARCHIVE);

    /*
     * Save?
     */
    public static readonly CVarDef<string> LastScenePrototype =
        CVarDef.Create("game.last_scene", "default");

    /*
     * Ui
     */
    public static readonly CVarDef<string> UIClickSound = CVarDef.Create("ui.clicksSound","/Audio/UserInterface/hover.ogg");
    public static readonly CVarDef<string> UIHoverSound = CVarDef.Create("ui.hoverSound","/Audio/UserInterface/hover.ogg");
    
    public static readonly CVarDef<string> BackroundMenu = CVarDef.Create("ui.backgroundMenu","/Textures/Interface/cat.jpg");
    public static readonly CVarDef<float> InterfaceVolume = CVarDef.Create("ui.interface_volume", 0.50f);
    
    /**
     * Game
     */
    
    public static readonly CVarDef<bool> GameLoadImmediately = CVarDef.Create("game.load_immediately", true);

}
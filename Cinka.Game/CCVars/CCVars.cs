using Robust.Shared;
using Robust.Shared.Configuration;

namespace Cinka.Game.CCVars;

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

    public static readonly CVarDef<int> ViewportWidth =
        CVarDef.Create("viewport.width", 16, CVar.CLIENTONLY | CVar.ARCHIVE);

    /*
     * Save?
     */
    public static readonly CVarDef<string> LastScenePrototype =
        CVarDef.Create("game.last_scene", "default");
}
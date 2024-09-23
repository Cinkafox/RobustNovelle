using Content.Game.Camera.Manager;
using Content.Game.Location.Managers;
using Content.Game.Scene.Manager;
using Content.Game.StyleSheet;
using Content.Game.Viewport;

namespace Content.Game;

public static class CiIoC
{
    public static void Register()
    {
        IoCManager.Register<ViewportManager, ViewportManager>();
        IoCManager.Register<StyleSheetManager, StyleSheetManager>();
        IoCManager.Register<ILocationManager, LocationManager>();
        IoCManager.Register<ICameraManager, CameraManager>();
        IoCManager.Register<ISceneManager, SceneManager>();
    }
}
using Content.Game.Scene.Manager;
using Content.Game.Viewport;

namespace Content.Game;

public static class CiIoC
{
    public static void Register()
    {
        IoCManager.Register<ViewportManager, ViewportManager>();
        IoCManager.Register<ISceneManager, SceneManager>();
    }
}
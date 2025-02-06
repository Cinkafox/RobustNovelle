using Content.Client.Scene.Manager;
using Content.Client.Viewport;

namespace Content.Client;

public static class CiIoC
{
    public static void Register()
    {
        IoCManager.Register<ViewportManager, ViewportManager>();
        IoCManager.Register<ISceneManager, SceneManager>();
    }
}
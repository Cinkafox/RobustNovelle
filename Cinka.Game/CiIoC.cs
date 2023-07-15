using Cinka.Game.Background.Manager;
using Cinka.Game.Viewport;
using Robust.Shared.IoC;

namespace Cinka.Game;

internal static class CiIoC
{
    public static void Register()
    {
        IoCManager.Register<ViewportManager, ViewportManager>();
        IoCManager.Register<IBackgroundManager,BackgroundManager>();
    }
}
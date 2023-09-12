using Robust.Client.ResourceManagement;
using Robust.Shared.IoC;

namespace Cinka.Game;

public static class StaticIoC
{
    public static IResourceCache ResC => IoCManager.Resolve<IResourceCache>();
}
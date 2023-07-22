using Cinka.Game.Audio.Managers;
using Cinka.Game.Background.Manager;
using Cinka.Game.Camera.Manager;
using Cinka.Game.Character.Managers;
using Cinka.Game.Location.Managers;
using Cinka.Game.Scene.Manager;
using Cinka.Game.StyleSheet;
using Cinka.Game.Viewport;
using Robust.Shared.IoC;

namespace Cinka.Game;

internal static class CiIoC
{
    public static void Register()
    {
        IoCManager.Register<ViewportManager, ViewportManager>();
        IoCManager.Register<IBackgroundManager,BackgroundManager>();
        IoCManager.Register<IStylesheetManager, StylesheetManager>();
        IoCManager.Register<ICharacterManager,CharacterManager>();
        IoCManager.Register<ILocationManager,LocationManager>();
        IoCManager.Register<ICameraManager,CameraManager>();
        IoCManager.Register<ISceneManager,SceneManager>();
        IoCManager.Register<IAudioManager,AudioManager>();
    }
}
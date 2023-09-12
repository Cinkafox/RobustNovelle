using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Cinka.Game.Audio.Managers;

public class AudioManager : IAudioManager
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    private AudioSystem _audio;

    private IPlayingAudioStream? _currentBackground;

    public void Initialize()
    {
        _audio = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<AudioSystem>();
        IoCManager.InjectDependencies(this);
    }

    public void PlayBackground(string path)
    {
        _currentBackground = _audio.PlayGlobal(path,
            _playerManager.LocalPlayer.Session, AudioParams.Default.WithLoop(true).WithVolume(-6));
    }

    public void StopBackground()
    {
        _currentBackground?.Stop();
    }

    public void Play(string path)
    {
        _audio.PlayGlobal(path,
            _playerManager.LocalPlayer.Session, AudioParams.Default.WithVolume(-4));
    }
}
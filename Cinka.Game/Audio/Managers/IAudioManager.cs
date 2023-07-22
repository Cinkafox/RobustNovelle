namespace Cinka.Game.Audio.Managers;

public interface IAudioManager
{
    public void Initialize();
    public void PlayBackground(string path);
    public void StopBackground();
    public void Play(string path);
}
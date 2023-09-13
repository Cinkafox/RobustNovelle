namespace Cinka.Game.Audio.Managers;

public interface IAudioManager
{
    public void Initialize();
    public void Shutdown();
    public void Play(string audioPrototype);
}
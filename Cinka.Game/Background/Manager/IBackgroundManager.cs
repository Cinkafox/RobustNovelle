using Robust.Client.Graphics;

namespace Cinka.Game.Background.Manager;

public interface IBackgroundManager
{
    Texture[] GetCurrentBackground();

    void LoadBackground(string name);
    void UnloadBackground();
}
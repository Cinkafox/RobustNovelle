using System;
using Robust.Client.Graphics;

namespace Cinka.Game.Background.Manager;

public interface IBackgroundManager
{
    Texture[] GetCurrentBackground();
    Texture[] GetFadingBackground();
    void ClearFadingBackground();
    bool TryGetFadingBackground(out Texture[] textures);
    TimeSpan GetLastFadingBackgroundUpdateCurTime();
    void LoadBackground(string name);
    void UnloadBackground();
}
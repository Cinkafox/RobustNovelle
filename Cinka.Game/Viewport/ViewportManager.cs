using System.Collections.Generic;
using Cinka.Game.UserInterface.Controls;

namespace Cinka.Game.Viewport;

public sealed class ViewportManager
{
    private readonly List<MainViewport> _viewports = new();

    private void UpdateCfg()
    {
        _viewports.ForEach(v => v.UpdateCfg());
    }

    public void AddViewport(MainViewport vp)
    {
        _viewports.Add(vp);
    }

    public void RemoveViewport(MainViewport vp)
    {
        _viewports.Remove(vp);
    }
}
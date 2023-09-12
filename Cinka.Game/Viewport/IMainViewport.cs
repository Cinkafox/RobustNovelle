using Cinka.Game.UserInterface.Controls;

namespace Cinka.Game.Viewport;

/// <summary>
///     Client state that has a main viewport.
/// </summary>
/// <remarks>
///     Used for taking no-UI screenshots (including things like flash overlay).
/// </remarks>
public interface IMainViewportState
{
    public MainViewport Viewport { get; }
}
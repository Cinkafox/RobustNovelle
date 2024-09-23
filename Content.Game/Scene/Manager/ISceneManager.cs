using Content.Game.Scene.Data;

namespace Content.Game.Scene.Manager;

public interface ISceneManager
{
    public void Initialize();
    public void LoadScene(string prototype);
    public ScenePrototype? GetCurrentScene();
    public void SaveScenePosition();
}
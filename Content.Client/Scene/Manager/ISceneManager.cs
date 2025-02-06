using Content.Client.Scene.Data;

namespace Content.Client.Scene.Manager;

public interface ISceneManager
{
    public void Initialize();
    public void LoadScene(string prototype);
    public ScenePrototype? GetCurrentScene();
    public void SaveScenePosition();
}
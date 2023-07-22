using Cinka.Game.Scene.Data;

namespace Cinka.Game.Scene.Manager;

public interface ISceneManager
{
    public void Initialize();
    public void LoadScene(string prototype);
    public void ContinueDialog();
    public SceneData? GetCurrentScenePrototype();
}
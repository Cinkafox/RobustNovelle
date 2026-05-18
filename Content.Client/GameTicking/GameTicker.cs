using Content.Client.Camera.Systems;
using Content.Client.Scene.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Player;

namespace Content.Client.GameTicking;

public sealed partial class GameTicker : EntitySystem
{
    [Dependency] private CameraSystem _cameraSystem = default!;
    [Dependency] private IConfigurationManager _configurationManager = default!;
    [Dependency] private SceneSystem _sceneSystem = default!;


    public void SpawnPlayer(ICommonSession session)
    {
        var uid = _cameraSystem.CreateCamera(session);
        _sceneSystem.LoadScene(uid, _configurationManager.GetCVar(CCVars.CCVars.LastScenePrototype));
    }
}
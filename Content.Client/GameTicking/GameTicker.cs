using Content.Client.Camera.Systems;
using Content.Client.Gameplay;
using Content.Client.Scene.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Player;

namespace Content.Client.GameTicking;

public sealed class GameTicker : EntitySystem
{
    [Dependency] private readonly SceneSystem _sceneSystem = default!;
    [Dependency] private readonly CameraSystem _cameraSystem = default!;
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;
    

    public void SpawnPlayer(ICommonSession session)
    {
        var uid = _cameraSystem.CreateCamera(session);
        _sceneSystem.LoadScene(uid, _configurationManager.GetCVar(CCVars.CCVars.LastScenePrototype));
    }
}
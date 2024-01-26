using System;
using System.Collections.Generic;
using Cinka.Game.Camera.Manager;
using Cinka.Game.Input;
using Cinka.Game.Interactable.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Cinka.Game.Interactable.Systems;

public sealed class InteractionSystem : EntitySystem
{
    [Dependency] private readonly ICameraManager _cameraManager = default!;
    public override void Initialize()
    {
        CommandBinds.Builder.Bind(ContentKeyFunctions.ActivateItemInWorld,
            new PointerInputCmdHandler(HandleActivateInWorld)).Register<InteractionSystem>();
    }

    private bool HandleActivateInWorld(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
    {
        RaiseLocalEvent(uid,new ActivateInWorldEvent(_cameraManager.GetCameraEntity(),uid));
        return false;
    }
}
using Content.Client.Camera.Components;
using Content.Client.Utils;
using Robust.Client.GameStates;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Controllers;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.Movement;

public sealed partial class InputMoverController : VirtualController
{
    [Dependency] private IClientGameStateManager _gameStateManager = default!;

    private EntityQuery<InputMoverComponent> _inputMoverQuery;

    public override void Initialize()
    {
        base.Initialize();
        _inputMoverQuery = GetEntityQuery<InputMoverComponent>();

        CommandBinds.Builder
            .Bind(EngineKeyFunctions.MoveUp, new MoverDirInputCmdHandler(this, DirectionFlag.North))
            .Bind(EngineKeyFunctions.MoveLeft, new MoverDirInputCmdHandler(this, DirectionFlag.West))
            .Bind(EngineKeyFunctions.MoveRight, new MoverDirInputCmdHandler(this, DirectionFlag.East))
            .Bind(EngineKeyFunctions.MoveDown, new MoverDirInputCmdHandler(this, DirectionFlag.South))
            .Bind(EngineKeyFunctions.Walk, new RunInputCmdHandler(this))
            .Register<InputMoverController>();
    }

    public void HandleDirChange(EntityUid sessionAttachedEntity, DirectionFlag direction, ushort messageSubTick,
        bool isDown)
    {
        if (!_inputMoverQuery.TryComp(sessionAttachedEntity, out var inputMoverComponent))
            return;

        if (isDown)
            inputMoverComponent.Direction |= direction;
        else
            inputMoverComponent.Direction &= ~direction;
    }

    public void HandleRunChange(EntityUid sessionAttachedEntity, ushort messageSubTick, bool isRunning)
    {
        if (!_inputMoverQuery.TryComp(sessionAttachedEntity, out var inputMoverComponent))
            return;

        inputMoverComponent.IsRunning = isRunning;
    }

    public override void UpdateBeforeSolve(bool prediction, float frameTime)
    {
        base.UpdateBeforeSolve(prediction, frameTime);
        _gameStateManager.ResetPredictedEntities();

        if (!IoCManager.Resolve<IGameTiming>().IsFirstTimePredicted) return;

        var query = EntityQueryEnumerator<InputMoverComponent, CameraComponent>();

        while (query.MoveNext(out var uid, out var inputMoverComponent, out var cameraComponent))
        {
            if (cameraComponent.FollowUid is null ||
                !TryComp<PhysicsComponent>(cameraComponent.FollowUid.Value, out var physicsComponent)) continue;

            if (inputMoverComponent.OldMagnitude == 0 && inputMoverComponent.Magnitude != 0)
                RaiseLocalEvent(cameraComponent.FollowUid.Value, new OnEntityMoving());
            if (inputMoverComponent.OldMagnitude != 0 && inputMoverComponent.Magnitude == 0)
                RaiseLocalEvent(cameraComponent.FollowUid.Value, new OnEntityStopMoving());

            PhysicsSystem.SetLinearVelocity(cameraComponent.FollowUid.Value,
                inputMoverComponent.MoveVelocity, body: physicsComponent);

            if (inputMoverComponent.IsMoving && inputMoverComponent.IsMoveEnabled)
                Transform(cameraComponent.FollowUid.Value).LocalRotation = inputMoverComponent.Direction.ToAngle();
            
            inputMoverComponent.OldMagnitude = inputMoverComponent.Magnitude;
        }
    }
}

public sealed class OnEntityMoving : EntityEventArgs
{
}

public sealed class OnEntityStopMoving : EntityEventArgs
{
}

public sealed class RunInputCmdHandler : InputCmdHandler
{
    private readonly InputMoverController _inputMoverController;

    public RunInputCmdHandler(InputMoverController inputMoverController)
    {
        _inputMoverController = inputMoverController;
    }

    public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session,
        IFullInputCmdMessage message)
    {
        if (session?.AttachedEntity is null) return false;
        _inputMoverController.HandleRunChange(session.AttachedEntity.Value, message.SubTick,
            message.State != BoundKeyState.Up);
        return false;
    }
}

public sealed class MoverDirInputCmdHandler : InputCmdHandler
{
    private readonly DirectionFlag _dir;
    private readonly InputMoverController _inputMoverController;

    public MoverDirInputCmdHandler(InputMoverController inputMoverController, DirectionFlag dir)
    {
        _inputMoverController = inputMoverController;
        _dir = dir;
    }

    public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session,
        IFullInputCmdMessage message)
    {
        if (session?.AttachedEntity is null) return false;

        _inputMoverController.HandleDirChange(session.AttachedEntity.Value, _dir, message.SubTick,
            message.State == BoundKeyState.Down);
        return false;
    }
}
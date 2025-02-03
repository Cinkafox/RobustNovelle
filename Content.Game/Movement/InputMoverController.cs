using System.Numerics;
using Content.Game.Camera.Components;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Controllers;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Game.Movement;

public sealed class InputMoverController : VirtualController
{
    private EntityQuery<InputMoverComponent> _inputMoverQuery;
    
    public override void Initialize()
    {
        base.Initialize();
        _inputMoverQuery = GetEntityQuery<InputMoverComponent>();
        
        CommandBinds.Builder
            .Bind(EngineKeyFunctions.MoveUp, new MoverDirInputCmdHandler(this, Direction.North))
            .Bind(EngineKeyFunctions.MoveLeft, new MoverDirInputCmdHandler(this, Direction.West))
            .Bind(EngineKeyFunctions.MoveRight, new MoverDirInputCmdHandler(this, Direction.East))
            .Bind(EngineKeyFunctions.MoveDown, new MoverDirInputCmdHandler(this, Direction.South))
            .Bind(EngineKeyFunctions.Walk, new RunInputCmdHandler(this))
            .Register<InputMoverController>();
    }

    public void HandleDirChange(EntityUid sessionAttachedEntity, Direction direction, ushort messageSubTick, bool isDown)
    {
        if(!_inputMoverQuery.TryComp(sessionAttachedEntity, out var inputMoverComponent))
            return;

        if (isDown)
        {
            inputMoverComponent.Direction = direction;
            inputMoverComponent.ButtonPressed += 1;
        }
        else
        {
            inputMoverComponent.ButtonPressed -= 1;
        }
    }

    public void HandleRunChange(EntityUid sessionAttachedEntity, ushort messageSubTick, bool isRunning)
    {
        if(!_inputMoverQuery.TryComp(sessionAttachedEntity, out var inputMoverComponent))
            return;
        
        inputMoverComponent.IsRunning = isRunning;
    }

    public override void UpdateBeforeSolve(bool prediction, float frameTime)
    {
        base.UpdateBeforeSolve(prediction, frameTime);
        
        if (!IoCManager.Resolve<IGameTiming>().IsFirstTimePredicted) return;
        
        var query = EntityQueryEnumerator<InputMoverComponent, CameraComponent>();

        while (query.MoveNext(out var uid, out var inputMoverComponent, out var cameraComponent))
        {
            if (inputMoverComponent.IsMoving)
            {
                inputMoverComponent.Speed += 0.1f;
            }
            else
            {
                inputMoverComponent.Speed -= 0.2f;
            }

            inputMoverComponent.Speed = float.Clamp(inputMoverComponent.Speed, 0, 3);
            
            if(cameraComponent.FollowUid is null ) continue;
            cameraComponent.FollowUid.Value.Comp.LocalPosition += inputMoverComponent.Direction.ToVec() * inputMoverComponent.Speed * frameTime;
            cameraComponent.FollowUid.Value.Comp.LocalRotation = inputMoverComponent.Direction.ToAngle();
        }
    }
}

public sealed class RunInputCmdHandler : InputCmdHandler
{
    private readonly InputMoverController _inputMoverController;

    public RunInputCmdHandler(InputMoverController inputMoverController)
    {
        _inputMoverController = inputMoverController;
    }

    public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
    {
        if (session?.AttachedEntity is null) return false;
        _inputMoverController.HandleRunChange(session.AttachedEntity.Value, message.SubTick, message.State == BoundKeyState.Up);
        return false;
    }
}

public sealed class MoverDirInputCmdHandler : InputCmdHandler
{
    private readonly InputMoverController _inputMoverController;
    private readonly Direction _dir;

    public MoverDirInputCmdHandler(InputMoverController inputMoverController, Direction dir)
    {
        _inputMoverController = inputMoverController;
        _dir = dir;
    }

    public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
    {
        if (session?.AttachedEntity is null) return false;
        
        _inputMoverController.HandleDirChange(session.AttachedEntity.Value, _dir, message.SubTick, message.State == BoundKeyState.Down);
        return false;
    }
}

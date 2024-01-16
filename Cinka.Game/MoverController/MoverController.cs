using System;
using System.Numerics;
using Cinka.Game.Camera;
using Cinka.Game.MoverController.Components;
using Robust.Client.Audio;
using Robust.Client.GameObjects;
using Robust.Client.Physics;
using Robust.Client.Player;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Controllers;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Cinka.Game.MoverController;

public sealed partial class MoverController : VirtualController
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IConfigurationManager _configManager = default!;
    [Dependency] private readonly IGameTiming Timing = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly ITileDefinitionManager _tileDefinitionManager = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly ContainerSystem _container = default!;
    [Dependency] private readonly MapSystem _mapSystem = default!;
    [Dependency] private readonly PhysicsSystem Physics = default!;
    [Dependency] private readonly TransformSystem _transform = default!;

    private EntityQuery<InputMoverComponent> MoverQuery;
    private EntityQuery<MobMoverComponent> MobMoverQuery;
    private EntityQuery<PhysicsComponent> PhysicsQuery;
    private EntityQuery<TransformComponent> XformQuery;
    private EntityQuery<CameraControlledComponent> cameraQuery;
    
    private float _stopSpeed = 0.1f;

    public override void Initialize()
    {
        base.Initialize();
        
        SubscribeLocalEvent<InputMoverComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<InputMoverComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);
        
        MoverQuery = GetEntityQuery<InputMoverComponent>();
        MobMoverQuery = GetEntityQuery<MobMoverComponent>();
        PhysicsQuery = GetEntityQuery<PhysicsComponent>();
        XformQuery = GetEntityQuery<TransformComponent>();
        cameraQuery = GetEntityQuery<CameraControlledComponent>();
        
        _transform.Reset();

        InitializeInput();
    }
    
    private void OnPlayerAttached(EntityUid uid, InputMoverComponent component, LocalPlayerAttachedEvent args)
    {
        SetMoveInput(component, MoveButtons.None);
    }

    private void OnPlayerDetached(EntityUid uid, InputMoverComponent component, LocalPlayerDetachedEvent args)
    {
        SetMoveInput(component, MoveButtons.None);
    }


    public override void UpdateBeforeSolve(bool prediction, float frameTime)
    {
        base.UpdateBeforeSolve(prediction, frameTime);

        var inputQueryEnumerator = AllEntityQuery<InputMoverComponent>();

        while (inputQueryEnumerator.MoveNext(out var uid, out var mover))
        {
            var physicsUid = uid;
            

            if (!XformQuery.TryGetComponent(uid, out var xform))
            {
                continue;
            }

            PhysicsComponent? body;
            var xformMover = xform;

            if (cameraQuery.TryGetComponent(uid, out var cameraControlledComponent))
            {
                continue;
            }

            if (mover.ToParent)
            {
                if (!PhysicsQuery.TryGetComponent(xform.ParentUid, out body) ||
                    !XformQuery.TryGetComponent(xform.ParentUid, out xformMover))
                {
                    continue;
                }
                
                physicsUid = xform.ParentUid;
            }
            else if (!PhysicsQuery.TryGetComponent(uid, out body))
            {
                continue;
            }

            HandleMobMovement(uid,
                mover,
                physicsUid,
                body,
                xformMover,
                frameTime);
        }
    }
    
    /// <summary>
    ///     Movement while considering actionblockers, weightlessness, etc.
    /// </summary>
    private void HandleMobMovement(
        EntityUid uid,
        InputMoverComponent mover,
        EntityUid physicsUid,
        PhysicsComponent physicsComponent,
        TransformComponent xform,
        float frameTime)
    {
        //LerpRotation(uid, mover, frameTime);
        
        var (walkDir, sprintDir) = GetVelocityInput(mover);

        var worldTotal = walkDir * 1 + sprintDir * 2;
        var worldRot = _transform.GetWorldRotation(xform);
        var velocity = physicsComponent.LinearVelocity;
        //Logger.Debug(velocity.ToString() + " " + physicsUid + " " + uid);

        Friction(0.01f, frameTime,10, ref velocity);
        
        if (worldTotal != Vector2.Zero)
        {
            _transform.SetLocalRotation(xform, xform.LocalRotation + worldTotal.ToWorldAngle() - worldRot);
        }
        
        Accelerate(ref velocity,worldTotal,10,frameTime);

        PhysicsSystem.SetLinearVelocity(physicsUid, velocity, body: physicsComponent);
        //TransformSystem.SetCoordinates(physicsUid,Transform(physicsUid).Coordinates.Offset(walkDir));
        
        PhysicsSystem.SetAngularVelocity(physicsUid, 0, body: physicsComponent);
    }
    
    private void Accelerate(ref Vector2 currentVelocity, in Vector2 velocity, float accel, float frameTime)
    {
        var wishDir = velocity != Vector2.Zero ? velocity.Normalized() : Vector2.Zero;
        var wishSpeed = velocity.Length();

        var currentSpeed = Vector2.Dot(currentVelocity, wishDir);
        var addSpeed = wishSpeed - currentSpeed;

        if (addSpeed <= 0f)
            return;

        var accelSpeed = accel * frameTime * wishSpeed;
        accelSpeed = MathF.Min(accelSpeed, addSpeed);

        currentVelocity += wishDir * accelSpeed;
    }
    
    public void LerpRotation(EntityUid uid, InputMoverComponent mover, float frameTime)
    {
        var angleDiff = Angle.ShortestDistance(mover.RelativeRotation, mover.TargetRelativeRotation);

        // if we've just traversed then lerp to our target rotation.
        if (!angleDiff.EqualsApprox(Angle.Zero, 0.001))
        {
            var adjustment = angleDiff * 5f * frameTime;
            var minAdjustment = 0.01 * frameTime;

            if (angleDiff < 0)
            {
                adjustment = Math.Min(adjustment, -minAdjustment);
                adjustment = Math.Clamp(adjustment, angleDiff, -angleDiff);
            }
            else
            {
                adjustment = Math.Max(adjustment, minAdjustment);
                adjustment = Math.Clamp(adjustment, -angleDiff, angleDiff);
            }

            mover.RelativeRotation += adjustment;
            mover.RelativeRotation.FlipPositive();
            Dirty(uid, mover);
        }
        else if (!angleDiff.Equals(Angle.Zero))
        {
            mover.TargetRelativeRotation.FlipPositive();
            mover.RelativeRotation = mover.TargetRelativeRotation;
            Dirty(uid, mover);
        }
    }
    
    private void Friction(float minimumFrictionSpeed, float frameTime, float friction, ref Vector2 velocity)
    {
        var speed = velocity.Length();

        if (speed < minimumFrictionSpeed)
            return;

        var drop = 0f;

        var control = MathF.Max(_stopSpeed, speed);
        drop += control * friction * frameTime;

        var newSpeed = MathF.Max(0f, speed - drop);

        if (newSpeed.Equals(speed))
            return;

        newSpeed /= speed;
        velocity *= newSpeed;
    }
}

[Flags]
[Serializable]
public enum MoveButtons : byte
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8,
    Walk = 16,
}
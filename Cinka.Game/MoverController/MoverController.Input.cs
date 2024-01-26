using System;
using System.Linq;
using System.Numerics;
using Cinka.Game.Dialog.Systems;
using Cinka.Game.Input;
using Cinka.Game.MoverController.Components;
using Cinka.Game.MoverController.Events;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Cinka.Game.MoverController;

public sealed partial class MoverController
{
    [Dependency] private readonly InputSystem _inputSystem = default!;
    private void InitializeInput()
    {
        var moveUpCmdHandler = new MoverDirInputCmdHandler(this, Direction.North);
        var moveLeftCmdHandler = new MoverDirInputCmdHandler(this, Direction.West);
        var moveRightCmdHandler = new MoverDirInputCmdHandler(this, Direction.East);
        var moveDownCmdHandler = new MoverDirInputCmdHandler(this, Direction.South);
        
        CommandBinds.Builder
            .Bind(EngineKeyFunctions.MoveUp, moveUpCmdHandler)
            .Bind(EngineKeyFunctions.MoveLeft, moveLeftCmdHandler)
            .Bind(EngineKeyFunctions.MoveRight, moveRightCmdHandler)
            .Bind(EngineKeyFunctions.MoveDown, moveDownCmdHandler)
            .Bind(EngineKeyFunctions.Walk, new WalkInputCmdHandler(this))
            .Register<MoverController>();
        

        SubscribeLocalEvent<InputMoverComponent, ComponentInit>(OnInputInit);
    }

    private void HandleDirChange(EntityUid entity, Direction dir, ushort subTick, bool state)
    {
        // Relayed movement just uses the same keybinds given we're moving the relayed entity
        // the same as us.

        if (!MoverQuery.TryGetComponent(entity, out var moverComp))
            return;

        SetVelocityDirection(entity, moverComp, dir, subTick, state);
    }

    private void OnInputInit(EntityUid uid, InputMoverComponent component, ComponentInit args)
    {
        var xform = Transform(uid);

        if (!xform.ParentUid.IsValid())
            return;

        component.RelativeEntity = xform.GridUid ?? xform.MapUid;
        component.TargetRelativeRotation = Angle.Zero;
    }
    
    public (Vector2 Walking, Vector2 Sprinting) GetVelocityInput(InputMoverComponent mover)
        {
            if (!Timing.InSimulation)
            {
                // Outside of simulation we'll be running client predicted movement per-frame.
                // So return a full-length vector as if it's a full tick.
                // Physics system will have the correct time step anyways.
                var immediateDir = DirVecForButtons(mover.HeldMoveButtons);
                return mover.Sprinting ? (Vector2.Zero, immediateDir) : (immediateDir, Vector2.Zero);
            }

            Vector2 walk;
            Vector2 sprint;
            float remainingFraction;

            if (Timing.CurTick > mover.LastInputTick)
            {
                walk = Vector2.Zero;
                sprint = Vector2.Zero;
                remainingFraction = 1;
            }
            else
            {
                walk = mover.CurTickWalkMovement;
                sprint = mover.CurTickSprintMovement;
                remainingFraction = (ushort.MaxValue - mover.LastInputSubTick) / (float) ushort.MaxValue;
            }

            var curDir = DirVecForButtons(mover.HeldMoveButtons) * remainingFraction;

            if (mover.Sprinting)
            {
                sprint += curDir;
            }
            else
            {
                walk += curDir;
            }

            // Logger.Info($"{curDir}{walk}{sprint}");
            return (walk, sprint);
        }

        /// <summary>
        ///     Toggles one of the four cardinal directions. Each of the four directions are
        ///     composed into a single direction vector, <see cref="VelocityDir"/>. Enabling
        ///     opposite directions will cancel each other out, resulting in no direction.
        /// </summary>
        public void SetVelocityDirection(EntityUid entity, InputMoverComponent component, Direction direction, ushort subTick, bool enabled)
        {
            // Logger.Info($"[{_gameTiming.CurTick}/{subTick}] {direction}: {enabled}");

            var bit = direction switch
            {
                Direction.East => MoveButtons.Right,
                Direction.North => MoveButtons.Up,
                Direction.West => MoveButtons.Left,
                Direction.South => MoveButtons.Down,
                _ => throw new ArgumentException(nameof(direction))
            };

            SetMoveInput(entity, component, subTick, enabled, bit);
        }

        private void SetMoveInput(EntityUid entity, InputMoverComponent component, ushort subTick, bool enabled, MoveButtons bit)
        {
            // Modifies held state of a movement button at a certain sub tick and updates current tick movement vectors.
            ResetSubtick(component);

            if (subTick >= component.LastInputSubTick)
            {
                var fraction = (subTick - component.LastInputSubTick) / (float) ushort.MaxValue;

                ref var lastMoveAmount = ref component.Sprinting ? ref component.CurTickSprintMovement : ref component.CurTickWalkMovement;

                lastMoveAmount += DirVecForButtons(component.HeldMoveButtons) * fraction;

                component.LastInputSubTick = subTick;
            }

            var buttons = component.HeldMoveButtons;

            if (enabled)
            {
                buttons |= bit;
            }
            else
            {
                buttons &= ~bit;
            }

            SetMoveInput(component, buttons);
        }
        
        protected void SetMoveInput(InputMoverComponent component, MoveButtons buttons)
        {
            if (component.HeldMoveButtons == buttons)
                return;
            var moveEvent = new MoveInputEvent(component.Owner, component, component.HeldMoveButtons);
            component.HeldMoveButtons = buttons;
            RaiseLocalEvent(component.Owner, ref moveEvent);
            Dirty(component.Owner, component);
        }
        
        private void HandleRunChange(EntityUid uid, ushort subTick, bool walking)
        {
            MoverQuery.TryGetComponent(uid, out var moverComp);

            if (moverComp == null) return;

            SetSprinting(uid, moverComp, subTick, walking);
        }


        private void ResetSubtick(InputMoverComponent component)
        {
            if (Timing.CurTick <= component.LastInputTick) return;

            component.CurTickWalkMovement = Vector2.Zero;
            component.CurTickSprintMovement = Vector2.Zero;
            component.LastInputTick = Timing.CurTick;
            component.LastInputSubTick = 0;
        }

        public void SetSprinting(EntityUid entity, InputMoverComponent component, ushort subTick, bool walking)
        {
            // Logger.Info($"[{_gameTiming.CurTick}/{subTick}] Sprint: {enabled}");

            SetMoveInput(entity, component, subTick, walking, MoveButtons.Walk);
        }
        
        /// <summary>
        ///     Retrieves the normalized direction vector for a specified combination of movement keys.
        /// </summary>
        private Vector2 DirVecForButtons(MoveButtons buttons)
        {
            // key directions are in screen coordinates
            // _moveDir is in world coordinates
            // if the camera is moved, this needs to be changed

            var x = 0;
            x -= HasFlag(buttons, MoveButtons.Left) ? 1 : 0;
            x += HasFlag(buttons, MoveButtons.Right) ? 1 : 0;

            var y = 0;
            //Diagonal shit
            if (x == 0)
            {
                y -= HasFlag(buttons, MoveButtons.Down) ? 1 : 0;
                y += HasFlag(buttons, MoveButtons.Up) ? 1 : 0;
            }

            var vec = new Vector2(x, y);

            // can't normalize zero length vector
            if (vec.LengthSquared() > 1.0e-6)
            {
                // Normalize so that diagonals aren't faster or something.
                vec = vec.Normalized();
            }

            return vec;
        }

        private static bool HasFlag(MoveButtons buttons, MoveButtons flag)
        {
            return (buttons & flag) == flag;
        }
               

        private sealed class MoverDirInputCmdHandler : InputCmdHandler
        {
            private readonly MoverController _controller;
            private readonly Direction _dir;

            public MoverDirInputCmdHandler(MoverController controller, Direction dir)
            {
                _controller = controller;
                _dir = dir;
            }

            public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
            {
                if (session?.AttachedEntity == null) return false;

                _controller.HandleDirChange(session.AttachedEntity.Value, _dir, message.SubTick, message.State == BoundKeyState.Down);
                return false;
            }
        }

        private sealed class WalkInputCmdHandler : InputCmdHandler
        {
            private MoverController _controller;

            public WalkInputCmdHandler(MoverController controller)
            {
                _controller = controller;
            }

            public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
            {
                if (session?.AttachedEntity == null) return false;

                _controller.HandleRunChange(session.AttachedEntity.Value, message.SubTick, message.State == BoundKeyState.Down);
                return false;
            }
        }
}
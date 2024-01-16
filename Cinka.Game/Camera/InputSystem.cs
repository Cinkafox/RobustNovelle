using System.Numerics;
using System.Reflection;
using Cinka.Game.Camera.Manager;
using Robust.Client.GameObjects;
using Robust.Client.Physics;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;

namespace Cinka.Game.Camera;

public sealed class InputSystem : EntitySystem
{
    [Dependency] private readonly ICameraManager _cameraManager = default!;
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly PhysicsSystem _physicsSystem = default!;

    public override void Initialize()
    {
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityManager.EntityQueryEnumerator<CameraComponent>();
        while (query.MoveNext(out var uid,out var comp))
        {
            if (TryComp<PhysicsComponent>(comp.AttachedEntity,out var body))
            {
               
            }
        }

    }
    
    
}


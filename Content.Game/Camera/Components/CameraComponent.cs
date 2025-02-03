namespace Content.Game.Camera.Components;

[RegisterComponent]
public sealed partial class CameraComponent : Component
{
   [ViewVariables] public Entity<TransformComponent>? FollowUid;
}
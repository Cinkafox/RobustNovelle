﻿namespace Content.Client.Camera.Components;

[RegisterComponent]
public sealed partial class CameraComponent : Component
{
   [ViewVariables] public EntityUid? FollowUid;
}
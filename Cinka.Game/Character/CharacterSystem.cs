using Cinka.Game.Character.Components;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Cinka.Game.Character;

public sealed class CharacterSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<CharacterComponent,ComponentInit>(OnComponentInit);
    }

    private void OnComponentInit(EntityUid uid, CharacterComponent component,ComponentInit args)
    {
        
    }
    
    
}
using Content.Client.Character.Systems;
using Content.Client.Dialog.Components;
using Content.Client.Dialog.Systems;
using Content.Client.Scene.Components;
using Content.Client.Scene.Data;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.Scene.Systems;

public sealed class SceneSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly ISerializationManager _serializationManager = default!;

    [Dependency] private readonly CharacterSystem _characterSystem = default!;
    [Dependency] private readonly DialogSystem _dialogSystem = default!;
    
    public void LoadScene(EntityUid actorUid, ProtoId<ScenePrototype> prototype)
    {
        if(!TryComp<SceneContainerComponent>(actorUid, out var container)) throw new Exception();
        LoadScene(new Entity<SceneContainerComponent>(actorUid, container), prototype);
    }
    
    public void LoadScene(Entity<SceneContainerComponent> entity, ProtoId<ScenePrototype> prototype)
    {
        CleanupScene(entity);

        if (!_prototypeManager.TryIndex(prototype, out var proto))
        {
            _cfg.SetCVar("game.last_scene", "default");
            throw new Exception($"Scene {prototype} not found!");
        }

        var dialogContainer = GetDialogContainer(entity);
        
        entity.Comp.CurrentScene = _serializationManager.CreateCopy(proto, notNullableOverride:true);
        
        foreach (var dialog in entity.Comp.CurrentScene.Dialogs) _dialogSystem.AddDialog(dialogContainer, dialog);
        
        _dialogSystem.ContinueDialog(dialogContainer);
    }

    public SceneData? GetCurrentScene(Entity<SceneContainerComponent> entity)
    {
        return entity.Comp.CurrentScene;
    }

    public void SaveScenePosition(Entity<SceneContainerComponent> entity)
    {
        if (entity.Comp.CurrentScene is ScenePrototype prototype) 
            _cfg.SetCVar(CCVars.CCVars.LastScenePrototype, prototype.ID);
    }

    public void CleanupScene(Entity<SceneContainerComponent> entity)
    {
        _dialogSystem.CleanupDialog(GetDialogContainer(entity));
        entity.Comp.CurrentScene = null;
    }

    private Entity<DialogContainerComponent> GetDialogContainer(Entity<SceneContainerComponent> entity)
    {
        return new Entity<DialogContainerComponent>(entity, Comp<DialogContainerComponent>(entity));
    }
}
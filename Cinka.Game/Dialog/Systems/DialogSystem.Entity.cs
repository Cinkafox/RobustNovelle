using Cinka.Game.Dialog.Components;
using Cinka.Game.Interactable.Events;
using Robust.Shared.GameObjects;

namespace Cinka.Game.Dialog.Systems;

public sealed partial class DialogSystem
{
    public void InitializeDialog()
    {
        SubscribeLocalEvent<DialogComponent,ActivateInWorldEvent>(OnActivate);
    }

    private void OnActivate(EntityUid uid, DialogComponent component, ActivateInWorldEvent args)
    {
        if(component.Dialog.HasValue) LoadDialog(component.Dialog.Value);
    }
}
using Content.Game.Dialog.Data;
using Content.Game.Dialog.Systems;
using Content.Game.Scene.Manager;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Game.Dialog.DialogActions.Dev;

public sealed partial class LoadEffectButtonAction : IDialogAction
{
    public void Act()
    {
        var entMan = IoCManager.Resolve<IEntityManager>();
        var sceneMan = IoCManager.Resolve<ISceneManager>();
        var e = IoCManager.Resolve<IEntityManager>().System<DialogSystem>();

        var dial = new Data.Dialog()
        {
            Text = "Выберите эффект"
        };

        var count = 0;

        foreach (var proto in IoCManager.Resolve<IPrototypeManager>().EnumeratePrototypes<AudioPresetPrototype>())
        {
            var btn = new DialogButton()
            {
                DialogAction = new SwitchEffectBackAction()
                {
                    Prototype = proto.ID
                },
                Name = proto.ID
            };
            dial.Choices.Add(btn);

            if (count % 5 == 0)
            {
                dial.Choices.Add(new DialogButton()
                {
                    Name = "Далее",
                    DialogAction = new DefaultDialogAction()
                });
                
                e.AddDialog(dial);
                
                dial = new Data.Dialog()
                {
                    Text = "Выберите эффект"
                };
            }

            count++;
        }
        
    }
}
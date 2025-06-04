using Content.Client.Camera.Components;
using Content.Client.Character.Components;
using Content.Client.Dialog.Components;
using Content.Client.Dialog.Data;
using Content.Client.Dialog.DialogActions;
using Robust.Client.Animations;
using Robust.Shared.Animations;

namespace Content.Client.Dialog.Systems;

public partial class DialogSystem
{
    private void LoadLocation(Entity<DialogContainerComponent> ent)
    {
        if(!TryComp<CameraComponent>(ent, out var camera)) return;
        
        var camEnt = new Entity<CameraComponent>(ent, camera);
        
        if (ent.Comp.CurrentDialog.Location is not null)
        {
            var locationUid = _location.LoadLocation(ent.Comp.CurrentDialog.Location);
            
            if(_location.TryGetLocationEntity(locationUid, ent.Comp.CameraFollowProtoId, out var camFol))
                _cameraSystem.FollowTo(camEnt, camFol);
            else
                _cameraSystem.FollowTo(camEnt, locationUid);
        }
        
        if(_location.TryGetLocationEntity(ent,ent.Comp.CurrentDialog.CameraOn, out var camFolWatch))
        {
            ent.Comp.CameraFollowProtoId = ent.Comp.CurrentDialog.CameraOn;
            _cameraSystem.FollowTo(camEnt, camFolWatch);
        }
    }

    private void SetTitle(Entity<DialogContainerComponent> ent)
    {
        if (ent.Comp.CurrentDialog.Title is not null)
        {
            _clyde.SetWindowTitle(ent.Comp.CurrentDialog.Title);
        }
    }

    private void EnsureDialogs(Entity<DialogContainerComponent> ent)
    {
        if (ent.Comp.CurrentDialog.NewDialog) _dialogUiController.ClearDialogs();
        
        if (IsEmptyString(ent.Comp.CurrentDialog.Text))
        {
            ent.Comp.CurrentDialog.IsDialog = false;
            if (ent.Comp.CurrentDialog.Choices.Count == 0)
            {
                ent.Comp.CurrentDialog.SkipDialog = true;
                _dialogUiController.Hide();
            }
        }
        else
        {
            _dialogUiController.Show();
        }
    }

    private void EnsureChoices(Entity<DialogContainerComponent> ent)
    {
        if (ent.Comp.CurrentDialog.Choices.Count == 0)
        {
            if (ent.Comp.CurrentDialog.SkipDialog)
            {
                ent.Comp.CurrentDialog.Actions.Add(new DefaultDialogAction());
            }
            else
                ent.Comp.CurrentDialog.Choices.Add(new DialogButton() { Name = Loc.GetString("dialog-continue") });
        }
    }

    private void ShowCharacters(Entity<DialogContainerComponent> ent)
    {
        if (ent.Comp.CurrentDialog.Show is not { } name) return;
        
        var spl = name.Split(" ");
        double? pos = spl.Length > 1 ? double.Parse(spl[1]) : null;
        
        if (!_characterSystem.TryGetCharacter(ent, spl[0], out var characterComponent, out var uid)) return;
        
        characterComponent.Visible = true;
        if (pos is null) return;
        
        if (characterComponent.XPosition == -1)
        {
            characterComponent.XPosition = pos.Value;
            return;
        }

        if (_animationPlayerSystem.HasRunningAnimation(uid, "xPos"))
            _animationPlayerSystem.Stop(uid, "xPos");

        _animationPlayerSystem.Play(uid, new Animation()
        {
            AnimationTracks =
            {
                new AnimationTrackComponentProperty()
                {
                    ComponentType = typeof(CharacterComponent),
                    Property = nameof(CharacterComponent.XPosition),
                    InterpolationMode = AnimationInterpolationMode.Cubic,
                    KeyFrames =
                    {
                        new AnimationTrackProperty.KeyFrame(characterComponent.XPosition, 0f),
                        new AnimationTrackProperty.KeyFrame(pos, 1f)
                    }
                }
            },
            Length = TimeSpan.FromSeconds(1f)
        }, "xPos");
    }

    private void HideCharacters(Entity<DialogContainerComponent> ent)
    {
        if (ent.Comp.CurrentDialog.Hide is { } aname && _characterSystem.TryGetCharacter(ent, aname, out var acharacterComponent, out _))
        {
            acharacterComponent.Visible = false;
        }
    }
}
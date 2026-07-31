using System.Text;
using Content.Client.Camera.Components;
using Content.Client.Character.Components;
using Content.Client.Dialog.Components;
using Robust.Client.Animations;
using Robust.Shared.Animations;

namespace Content.Client.Dialog.Systems;

public partial class DialogSystem
{
    private void CheckTweaksOfText(Entity<DialogContainerComponent> ent, Data.Dialog dialog)
    {
        var text = dialog.Text?.ToString();
        
        if(string.IsNullOrEmpty(text)) return;

        if (text[0] == '>')
        {
            text = text.Substring(1);
            dialog.NewDialog = false;
        }

        if (text[0] == '!')
        {
            text = text.Substring(1);
            dialog.DontLetSkip = true;
        }
        
        dialog.Text = text;
    }
    
    private void LoadLocation(Entity<DialogContainerComponent> ent, Data.Dialog dialog)
    {
        if (!TryComp<CameraComponent>(ent, out var camera)) return;

        var camEnt = new Entity<CameraComponent>(ent, camera);

        if (dialog.Location is not null)
        {
            var locationUid = _location.LoadLocation(dialog.Location.Value);

            if (_location.TryGetLocationEntity(locationUid, ent.Comp.CameraFollowProtoId, out var camFol))
                _cameraSystem.FollowTo(camEnt, camFol);
            else
                _cameraSystem.FollowTo(camEnt, locationUid);
        }

        if (_location.TryGetLocationEntity(ent, dialog.CameraOn, out var camFolWatch))
        {
            ent.Comp.CameraFollowProtoId = dialog.CameraOn;
            _cameraSystem.FollowTo(camEnt, camFolWatch);
        }
    }

    private void SetTitle(Entity<DialogContainerComponent> ent, Data.Dialog dialog)
    {
        if (dialog.Title is not null) _clyde.SetWindowTitle(dialog.Title);
    }

    private void EnsureDialogs(Entity<DialogContainerComponent> ent, Data.Dialog dialog)
    {
        if (dialog.NewDialog) _dialogUiController.ClearDialogs();
        
        if (string.IsNullOrEmpty(dialog.Text?.ToString()))
        {
            dialog.ShowEmotes = false;
            if (dialog.Choices.Count == 0)
            {
                dialog.SkipDialog = true;
                _dialogUiController.Hide();
            }
        }
        else
        {
            _dialogUiController.Show();
        }
    }

    private void ShowCharacters(Entity<DialogContainerComponent> ent, Data.Dialog dialog)
    {
        if (dialog.Show is not { } name) return;

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

        _animationPlayerSystem.Play(uid, new Animation
        {
            AnimationTracks =
            {
                new AnimationTrackComponentProperty
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

    private void HideCharacters(Entity<DialogContainerComponent> ent, Data.Dialog dialog)
    {
        if (dialog.Hide is { } aname &&
            _characterSystem.TryGetCharacter(ent, aname, out var acharacterComponent, out _))
            acharacterComponent.Visible = false;
    }
}
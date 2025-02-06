using Content.Client.Character.Components;
using Content.Client.Dialog.Data;
using Content.Client.Dialog.DialogActions;
using Robust.Client.Animations;
using Robust.Shared.Animations;

namespace Content.Client.Dialog.Systems;

public partial class DialogSystem
{
    private void LoadLocation()
    {
        if (CurrentDialog.Location is not null)
        {
            var location = _location.LoadLocation(CurrentDialog.Location);
            if (!_cameraSystem.ResetFollowing() && location.IsValid())
                _cameraSystem.FollowTo(location);
        }
        
        if (CurrentDialog.CameraOn is not null)
            _cameraSystem.FollowTo(CurrentDialog.CameraOn);
    }

    private void SetTitle()
    {
        if (CurrentDialog.Title is not null)
        {
            _clyde.SetWindowTitle(CurrentDialog.Title);
        }
    }

    private void EnsureDialogs()
    {
        if (CurrentDialog.NewDialog) _dialogUiController.ClearDialogs();
        
        if (IsEmptyString(CurrentDialog.Text))
        {
            CurrentDialog.IsDialog = false;
            if (CurrentDialog.Choices.Count == 0)
                CurrentDialog.SkipDialog = true;
        }
    }

    private void EnsureChoices()
    {
        if (CurrentDialog.Choices.Count == 0)
        {
            if (CurrentDialog.SkipDialog)
                CurrentDialog.Actions.Add(new DefaultDialogAction());
            else
                CurrentDialog.Choices.Add(new DialogButton() { Name = Loc.GetString("dialog-continue") });
        }
    }

    private void ShowCharacters()
    {
        if (CurrentDialog.Show is not { } name) return;
        
        var spl = name.Split(" ");
        double? pos = spl.Length > 1 ? double.Parse(spl[1]) : null;
        
        if (!_characterSystem.TryGetCharacter(spl[0], out var characterComponent, out var uid)) return;
        
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

    private void HideCharacters()
    {
        if (CurrentDialog.Hide is { } aname && _characterSystem.TryGetCharacter(aname, out var acharacterComponent, out _))
        {
            acharacterComponent.Visible = false;
        }
    }
}
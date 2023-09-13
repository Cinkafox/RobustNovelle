using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cinka.Game.Audio.Data;
using Robust.Client.Audio;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Audio;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Cinka.Game.Audio.Managers;

public sealed class AudioManager : IAudioManager
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IClydeAudio _clyde = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom RandMan = default!;
    private ISawmill _sawmill = default!;

    private AudioSystem.PlayingStream? _currentBackground;
    
    private readonly List<AudioSystem.PlayingStream> _playingClydeStreams = new();

    public void Initialize()
    {
        _sawmill = Logger.GetSawmill("customAudio");
        IoCManager.InjectDependencies(this);
    }

    public void Shutdown()
    {
        _currentBackground?.Source.Dispose();
        foreach (var stream in _playingClydeStreams)
        {
            stream.Source.Dispose();
        }
        _playingClydeStreams.Clear();
    }
    

    public void Play(string audioPrototype)
    {
        if(!_prototypeManager.TryIndex<AudioPrototype>(audioPrototype,out var prototype))
            return;

        Play(prototype.AudioStream,AudioParams.Default,isBackground: prototype.IsBackground);
    }
    
    
    private IPlayingAudioStream? Play(AudioStream stream, AudioParams audioParams, bool isBackground = false)
    {
        if (!TryCreateAudioSource(stream, out var source))
        {
            _sawmill.Error($"Error setting up global audio for {stream.Name}: {0}", Environment.StackTrace);
            return null;
        }

        source.SetGlobal();

        return CreateAndStartPlayingStream(source, audioParams,isBackground);
    }
    
    private bool TryCreateAudioSource(AudioStream stream, [NotNullWhen(true)] out IClydeAudioSource? source)
    {
        if (!_timing.IsFirstTimePredicted)
        {
            source = null;
            _sawmill.Error($"Tried to create audio source outside of prediction!");
            DebugTools.Assert(false);
            return false;
        }

        source = _clyde.CreateAudioSource(stream);
        return source != null;
    }
    
    private AudioSystem.PlayingStream CreateAndStartPlayingStream(IClydeAudioSource source, AudioParams audioParams, 
        bool isBackground = false)
    {
        if(isBackground)
        {
            _currentBackground?.Source.Dispose();
            audioParams = audioParams.WithLoop(true).WithVolume(-6);
        }

        ApplyAudioParams(audioParams, source);
        source.StartPlaying();
        var playing = new AudioSystem.PlayingStream
        {
            Source = source,
            Attenuation = audioParams.Attenuation,
            MaxDistance = audioParams.MaxDistance,
            ReferenceDistance = audioParams.ReferenceDistance,
            RolloffFactor = audioParams.RolloffFactor,
            Volume = audioParams.Volume
        };
        
        if (isBackground)
            _currentBackground = playing;
        else
            _playingClydeStreams.Add(playing);
        
        return playing;
    }
    
    private void ApplyAudioParams(AudioParams? audioParams, IClydeAudioSource source)
    {
        if (!audioParams.HasValue)
            return;

        if (audioParams.Value.Variation.HasValue)
            source.SetPitch(audioParams.Value.PitchScale
                            * (float) RandMan.NextGaussian(1, audioParams.Value.Variation.Value));
        else
            source.SetPitch(audioParams.Value.PitchScale);

        source.SetVolume(audioParams.Value.Volume);
        source.SetRolloffFactor(audioParams.Value.RolloffFactor);
        source.SetMaxDistance(audioParams.Value.MaxDistance);
        source.SetReferenceDistance(audioParams.Value.ReferenceDistance);
        source.SetPlaybackPosition(audioParams.Value.PlayOffsetSeconds);
        source.IsLooping = audioParams.Value.Loop;
    }
}
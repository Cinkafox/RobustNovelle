using Robust.Client.Audio;
using Robust.Shared.Utility;

namespace Cinka.Game.Audio.Data;

public sealed class AudioSpecifier
{
    public ResPath Path { get; private set; }
    public AudioStream AudioStream { get; private set; }
    
    public AudioSpecifier(AudioStream audioStream,ResPath path)
    {
        AudioStream = audioStream;
        Path = path;
    }
    
    public static implicit operator AudioStream(AudioSpecifier res)
    {
        return res.AudioStream;
    }
}
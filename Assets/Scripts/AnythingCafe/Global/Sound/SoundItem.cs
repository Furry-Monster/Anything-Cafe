using JetBrains.Annotations;
using UnityEngine;

public enum SoundType
{
    Ambient,
    Ero,
    Music,
    None,
    UI,
}

public record SoundItem(SoundType SoundType, AudioClip AudioClip, bool Loop, float Volume, ulong Delay)
{
    public SoundType SoundType { get; set; } = SoundType;

    public AudioClip AudioClip { get; set; } = AudioClip;

    public bool Loop { get; set; } = Loop;

    public float Volume { get; set; } = Volume;

    public ulong Delay { get; set; } = Delay;
}

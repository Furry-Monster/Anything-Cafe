using UnityEngine;
using UnityEngine.Audio;

public class SoundItem
{
    public SoundType SoundType { get; set; }
    public AudioClip AudioClip { get; set; }
    public bool Loop { get; set; }
    public float Volume { get; set; }
    public float Pitch { get; set; }
    public float SpatialBlend { get; set; }
    public ulong Delay { get; set; }
    public AudioMixerGroup OutputAudioMixerGroup { get; set; }
    public Vector3? Position { get; set; }

    public SoundItem(
        SoundType soundType,
        AudioClip audioClip,
        bool loop = false,
        float volume = 1.0f,
        float pitch = 1.0f,
        float spatialBlend = 0f,
        ulong delay = 0,
        AudioMixerGroup outputAudioMixerGroup = null,
        Vector3? position = null)
    {
        SoundType = soundType;
        AudioClip = audioClip;
        Loop = loop;
        Volume = volume;
        Pitch = pitch;
        SpatialBlend = spatialBlend;
        Delay = delay;
        OutputAudioMixerGroup = outputAudioMixerGroup;
        Position = position;
    }

    public void ApplyToSource(AudioSource source)
    {
        source.clip = AudioClip;
        source.loop = Loop;
        source.volume = Volume;
        source.pitch = Pitch;
        source.spatialBlend = SpatialBlend;
        source.outputAudioMixerGroup = OutputAudioMixerGroup;

        if (Position.HasValue)
        {
            source.transform.position = Position.Value;
        }
    }
}

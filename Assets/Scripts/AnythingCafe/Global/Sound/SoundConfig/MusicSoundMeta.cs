using UnityEngine;

[CreateAssetMenu(menuName = "FrameMonster/Audio/MusicSound")]
public class MusicSoundMeta : ScriptableObject
{
    public readonly SoundType Type = SoundType.Music;
    public AudioClip Clip;
    public bool Loop = true;
    [Range(0, 1)] public float DefaultVolume = 0.8f;
    [Range(0, 1)] public float DefaultPitch = 1f;
}
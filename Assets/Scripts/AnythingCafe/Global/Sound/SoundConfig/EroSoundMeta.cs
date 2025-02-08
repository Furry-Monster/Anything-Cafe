using UnityEngine;

[CreateAssetMenu(menuName = "FrameMonster/Audio/EroSound")]
public class EroSoundMeta : ScriptableObject
{
    public readonly SoundType Type = SoundType.Ero;
    public AudioClip Clip;
    public bool Loop;
    [Range(0, 1)] public float DefaultVolume = 0.8f;
    [Range(0, 1)] public float DefaultPitch = 1f;
}
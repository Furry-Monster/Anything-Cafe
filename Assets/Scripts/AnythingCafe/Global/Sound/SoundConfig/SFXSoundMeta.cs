using UnityEngine;

[CreateAssetMenu(menuName = "AnythingCafe/Audio/SFXSound")]
public class SFXSoundMeta : ScriptableObject
{
    public readonly SoundType Type = SoundType.SFX;
    public AudioClip Clip;
    [Range(0, 1)] public float DefaultVolume = 0.8f;
    [Range(0, 1)] public float DefaultPitch = 1f;
}
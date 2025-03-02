using UnityEngine;

[CreateAssetMenu(menuName = "AnythingCafe/Audio/AmbientSound")]
public class AmbientSoundMeta : ScriptableObject
{
    public readonly SoundType Type = SoundType.Ambient;
    public AudioClip Clip;
    public bool Loop = true;
    [Range(0, 1)] public float DefaultVolume = 0.8f;
    [Range(0, 1)] public float DefaultPitch = 1f;
}
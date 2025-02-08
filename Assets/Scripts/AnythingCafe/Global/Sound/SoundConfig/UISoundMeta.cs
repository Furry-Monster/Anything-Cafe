using UnityEngine;

[CreateAssetMenu(menuName = "FrameMonster/Audio/UISound")]
public class UISoundMeta : ScriptableObject
{
    public readonly SoundType SoundType = SoundType.UI;
    public AudioClip ClickSound;
    public AudioClip HoverSound;
    [Range(0, 1)] public float DefaultVolume = 0.8f;
    [Range(0, 1)] public float DefaultPitch = 1f;
}
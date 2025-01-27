using UnityEngine;

public enum SoundType
{
    None,
    Music,
    Ero,
    Ambient,
    UI,
}

public class SoundItem : MonoBehaviour
{
    public SoundType SoundType { get; set; }

    public AudioClip AudioClip { get; set; }
}

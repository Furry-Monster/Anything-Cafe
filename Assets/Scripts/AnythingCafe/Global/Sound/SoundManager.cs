using UnityEngine;



public class SoundManager : MonoBehaviourSingleton<SoundManager>
{
    private SoundPool _soundPool;

    [SerializeField]
    private SerializableDictionary<string, SerializableKeyValuePair<SoundType, AudioClip>> _soundItemDict;

    public GameObject SourceParent;

    protected override void Awake()
    {
        base.Awake();

        // ≥ı ºªØSoundPool
        if (SourceParent == null) SourceParent = this.gameObject;
        _soundPool ??= SoundPool.Instance.Initialize(SourceParent);
    }
}

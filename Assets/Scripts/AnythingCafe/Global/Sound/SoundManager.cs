using UnityEngine;

/// <summary>
/// ����������
/// </summary>
public class SoundManager : PersistentSingleton<SoundManager>
{
    private SoundPool _soundPool;

    [SerializeField]
    private SerializableDictionary<string, SerializableKeyValuePair<SoundType, AudioClip>> _soundItemDict;

    public GameObject SourceParent;

    protected override void Awake()
    {
        base.Awake();

        // ��ʼ��SoundPool
        if (SourceParent == null) SourceParent = this.gameObject;
        _soundPool ??= SoundPool.Instance.Initialize(SourceParent);
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="soundName"> �������� </param>
    /// <param name="loop"> �Ƿ�ѭ�� </param>
    /// <param name="volume"> ���� </param>
    /// <param name="delay"> �ӳٲ��� </param>
    public void PlaySound(string soundName, bool loop = false, float volume = 1.0f, ulong delay = 0ul)
    {
        if (_soundItemDict.TryGetValue(soundName, out var soundKvp))
        {
            // ���Һ��װһ��
            var soundItem = new SoundItem(soundKvp.Key, soundKvp.Value, loop, volume, delay);
            _soundPool.PlaySound(soundItem);
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[SoundManager] Missing audio item for {soundName}, please check the SoundManager on {gameObject.name}");
#endif
        }
    }

    /// <summary>
    /// ֹͣ����
    /// </summary>
    /// <param name="soundName"> �������� </param>
    public void StopSound(string soundName)
    {
        if (_soundItemDict.TryGetValue(soundName, out var soundKvp))
        {
            // ���Һ��װһ��
            var soundItem = new SoundItem(soundKvp.Key, soundKvp.Value,false,0,0);
            _soundPool.StopSound(soundItem);
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[SoundManager] Missing audio item for {soundName}, please check the SoundManager on {gameObject.name}");
#endif
        }
    }

    /// <summary>
    ///  ֹͣ��������
    /// </summary>
    public void StopAllSounds() => _soundPool.StopAllSounds();
}

using UnityEngine;

/// <summary>
/// ����������
/// </summary>
[AddComponentMenu("FrameMonster/Sound/SoundManager")]
public class SoundManager : PersistentSingleton<SoundManager>, IInitializable
{
    private SoundPool _soundPool;

    [SerializeField]
    private SerializableDictionary<string, SerializableKeyValuePair<SoundType, AudioClip>> _soundItemDict;

    public GameObject SourceParent; // ����Source��Ӧ��GameObject�ĸ�����

    public void Init()
    {
        // ��ʼ������SoundPool
        if (SourceParent == null) SourceParent = this.gameObject;
        _soundPool ??= SoundPool.Instance.Created(SourceParent);
    }

    #region ��������,�������š�ֹͣ����ͣ���ָ�
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
            var soundItem = new SoundItem(soundKvp.Key, soundKvp.Value, false, 0, 0);
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
    /// ֹͣ��������
    /// </summary>
    public void StopAllSounds() => _soundPool.StopAllSounds();

    /// <summary>
    /// ��ͣ����
    /// </summary>
    /// <param name="soundName"> �������� </param>
    public void PauseSound(string soundName)
    {
        if (_soundItemDict.TryGetValue(soundName, out var soundKvp))
        {
            // ���Һ��װһ��
            var soundItem = new SoundItem(soundKvp.Key, soundKvp.Value, false, 0, 0);
            _soundPool.PauseSound(soundItem);
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[SoundManager] Missing audio item for {soundName}, please check the SoundManager on {gameObject.name}");
#endif
        }
    }

    /// <summary>
    /// �ָ�����
    /// </summary>
    /// <param name="soundName"> �������� </param>
    public void ResumeSound(string soundName)
    {
        if (_soundItemDict.TryGetValue(soundName, out var soundKvp))
        {
            // ���Һ��װһ��
            var soundItem = new SoundItem(soundKvp.Key, soundKvp.Value, false, 0, 0);
            _soundPool.ResumeSound(soundItem);
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[SoundManager] Missing audio item for {soundName}, please check the SoundManager on {gameObject.name}");
#endif
        }
    }
    #endregion
}

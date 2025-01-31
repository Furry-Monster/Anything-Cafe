using UnityEngine;

/// <summary>
/// 声音管理器
/// </summary>
[AddComponentMenu("FrameMonster/Sound/SoundManager")]
public class SoundManager : PersistentSingleton<SoundManager>, IInitializable
{
    private SoundPool _soundPool;

    [SerializeField]
    private SerializableDictionary<string, SerializableKeyValuePair<SoundType, AudioClip>> _soundItemDict;

    public GameObject SourceParent; // 所有Source对应的GameObject的父物体

    public void Init()
    {
        // 初始化创建SoundPool
        if (SourceParent == null) SourceParent = this.gameObject;
        _soundPool ??= SoundPool.Instance.Created(SourceParent);
    }

    #region 声音操作,包括播放、停止、暂停、恢复
    /// <summary>
    /// 播放声音
    /// </summary>
    /// <param name="soundName"> 声音名称 </param>
    /// <param name="loop"> 是否循环 </param>
    /// <param name="volume"> 音量 </param>
    /// <param name="delay"> 延迟播放 </param>
    public void PlaySound(string soundName, bool loop = false, float volume = 1.0f, ulong delay = 0ul)
    {
        if (_soundItemDict.TryGetValue(soundName, out var soundKvp))
        {
            // 查找后包装一个
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
    /// 停止声音
    /// </summary>
    /// <param name="soundName"> 声音名称 </param>
    public void StopSound(string soundName)
    {
        if (_soundItemDict.TryGetValue(soundName, out var soundKvp))
        {
            // 查找后包装一个
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
    /// 停止所有声音
    /// </summary>
    public void StopAllSounds() => _soundPool.StopAllSounds();

    /// <summary>
    /// 暂停声音
    /// </summary>
    /// <param name="soundName"> 声音名称 </param>
    public void PauseSound(string soundName)
    {
        if (_soundItemDict.TryGetValue(soundName, out var soundKvp))
        {
            // 查找后包装一个
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
    /// 恢复声音
    /// </summary>
    /// <param name="soundName"> 声音名称 </param>
    public void ResumeSound(string soundName)
    {
        if (_soundItemDict.TryGetValue(soundName, out var soundKvp))
        {
            // 查找后包装一个
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

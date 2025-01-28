using UnityEngine;

/// <summary>
/// 声音管理器
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

        // 初始化SoundPool
        if (SourceParent == null) SourceParent = this.gameObject;
        _soundPool ??= SoundPool.Instance.Initialize(SourceParent);
    }

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
    ///  停止所有声音
    /// </summary>
    public void StopAllSounds() => _soundPool.StopAllSounds();
}

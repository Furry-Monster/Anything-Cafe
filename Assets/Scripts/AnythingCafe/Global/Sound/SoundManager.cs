using UnityEngine;

/// <summary>
/// 声音管理器
/// </summary>
[AddComponentMenu("FrameMonster/Sound/SoundManager")]
public class SoundManager : PersistentSingleton<SoundManager>, IInitializable
{
    private SoundPool _soundPool;

    [SerializeField]
    private GameObject _sourceParent; // 所有Source对应的GameObject的父物体

    public bool IsInitialized { get; set; }

    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        // 初始化创建SoundPool
        if (_sourceParent == null) _sourceParent = this.gameObject;
        _soundPool ??= SoundPool.Instance.Created(_sourceParent);
    }

    #region 声音操作,包括播放、停止、暂停、恢复

    /// <summary>
    /// 播放声音
    /// </summary>
    /// <param name="type"> 声音类型 </param>
    /// <param name="clip"> 音频Clip </param>
    /// <param name="loop"> 是否循环 </param>
    /// <param name="volume"> 音量 </param>
    /// <param name="delay"> 延迟播放 </param>
    public void PlaySound(SoundType type, AudioClip clip, bool loop = false, float volume = 1.0f, ulong delay = 0ul)
    {
        // 查找后包装一个
        var soundItem = new SoundItem(type, clip, loop, volume, delay);
        Debug.Log($"[SoundManager] Play sound [{type}]{clip.name} with loop {loop} and volume {volume} with delay {delay}ms");
        _soundPool.PlaySound(soundItem);
    }

    /// <summary>
    /// 停止声音
    /// </summary>
    /// <param name="type"> 声音类型 </param>
    /// <param name="clip"> 音频Clip </param>
    public void StopSound(SoundType type, AudioClip clip)
    {

        // 查找后包装一个
        var soundItem = new SoundItem(type, clip, false, 0, 0);
        _soundPool.StopSound(soundItem);
    }

    /// <summary>
    /// 停止所有声音
    /// </summary>
    public void StopAllSounds() => _soundPool.StopAllSounds();

    /// <summary>
    /// 暂停声音
    /// </summary>
    /// <param name="type"> 声音类型 </param>
    /// <param name="clip"> 音频Clip </param>
    public void PauseSound(SoundType type, AudioClip clip)
    {
        // 查找后包装一个
        var soundItem = new SoundItem(type, clip, false, 0, 0);
        _soundPool.PauseSound(soundItem);

    }

    /// <summary>
    /// 恢复声音
    /// </summary>
    /// <param name="type"> 声音类型 </param>
    /// <param name="clip"> 音频Clip </param>
    public void ResumeSound(SoundType type, AudioClip clip)
    {

        // 查找后包装一个
        var soundItem = new SoundItem(type, clip, false, 0, 0);
        _soundPool.ResumeSound(soundItem);
    }
    #endregion

}

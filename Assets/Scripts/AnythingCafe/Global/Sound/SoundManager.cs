using System;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 音频管理器
/// </summary>
[AddComponentMenu("FrameMonster/Sound/SoundManager")]
public class SoundManager : PersistentSingleton<SoundManager>, IInitializable
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private GameObject _sourceParent;

    private SoundPoolManager _soundPoolManager;
    private VolumeController _volumeController;

    public bool IsInitialized { get; set; }

    public void Init()
    {
        if (IsInitialized)
        {
            Debug.LogWarning("[SoundManager] SoundManager has already been initialized!");
            return;
        }

        try
        {
            // 初始化音频池
            _sourceParent ??= gameObject;
            _soundPoolManager = new SoundPoolManager(_sourceParent, _audioMixer);

            // 初始化音量控制器
            _volumeController = new VolumeController(_audioMixer);

            // 加载音频设置
            LoadOptions(OptionGroup.Audio);

            // 订阅设置变更事件
            OptionManager.Instance.OnOptionChanged += keyEnum => LoadOptions(keyEnum);

            IsInitialized = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SoundManager] Failed to initialize SoundManager: {e}");
            IsInitialized = false;
        }
    }

    private void LoadOptions(Enum optionEnum)
    {
        if (!IsInitialized) return;
        _volumeController.LoadOptions(optionEnum);
    }

    #region 公共方法
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="type">音效类型</param>
    /// <param name="clip">音效片段</param>
    /// <param name="loop">是否循环播放</param>
    /// <param name="volume">音量</param>
    /// <param name="delay">延迟播放时间(毫秒)</param>
    public void PlaySound(SoundType type, AudioClip clip, bool loop = false, float volume = 1.0f, ulong delay = 0)
    {
        if (!IsInitialized)
        {
            Debug.LogError("[SoundManager] SoundManager has not been initialized");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("[SoundManager] Trying to play an empty AudioClip");
            return;
        }

        try
        {
            var finalVolume = _volumeController.GetFinalVolume(type, volume);
            var soundItem = new SoundItem(type, clip, loop, finalVolume, 1f, 0f, delay);
            _soundPoolManager.PlaySound(soundItem);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SoundManager] Failed to play sound: {e}");
        }
    }

    /// <summary>
    /// 在指定位置播放音效
    /// </summary>
    /// <param name="type">音效类型</param>
    /// <param name="clip">音效片段</param>
    /// <param name="position">位置</param>
    /// <param name="loop">是否循环播放</param>
    /// <param name="volume">音量</param>
    /// <param name="delay">延迟播放时间(毫秒)</param>
    public void PlaySoundAtPosition(SoundType type, AudioClip clip, Vector3 position, bool loop = false, float volume = 1.0f, ulong delay = 0)
    {
        if (!IsInitialized)
        {
            Debug.LogError("[SoundManager] SoundManager has not been initialized");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("[SoundManager] Trying to play an empty AudioClip");
            return;
        }

        try
        {
            var finalVolume = _volumeController.GetFinalVolume(type, volume);
            var soundItem = new SoundItem(
                type, clip, loop, finalVolume, 1f, 1f, delay, null, position);
            _soundPoolManager.PlaySound(soundItem);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SoundManager] Failed to play sound at position: {e}");
        }
    }

    /// <summary>
    /// 停止音效
    /// </summary>
    /// <param name="type">音效类型</param>
    /// <param name="clip">音效片段</param>
    public void StopSound(SoundType type, AudioClip clip)
    {
        if (!IsInitialized)
        {
            Debug.LogError("[SoundManager] SoundManager has not been initialized");
            return;
        }

        try
        {
            _soundPoolManager.StopSound(new SoundItem(type, clip));
        }
        catch (Exception e)
        {
            Debug.LogError($"[SoundManager] Failed to stop sound: {e}");
        }
    }

    /// <summary>
    /// 停止所有音效
    /// </summary>
    public void StopAllSounds()
    {
        if (!IsInitialized)
        {
            Debug.LogError("[SoundManager] Sound Manager has not been initialized");
            return;
        }

        try
        {
            _soundPoolManager.StopAllSounds();
        }
        catch (Exception e)
        {
            Debug.LogError($"[SoundManager] Failed to stop all sounds: {e}");
        }
    }

    /// <summary>
    /// 暂停音效
    /// </summary>
    /// <param name="type">音效类型</param>
    /// <param name="clip">音效片段</param>
    public void PauseSound(SoundType type, AudioClip clip)
    {
        if (!IsInitialized)
        {
            Debug.LogError("[SoundManager] Sound Manager has not been initialized");
            return;
        }

        try
        {
            _soundPoolManager.PauseSound(new SoundItem(type, clip));
        }
        catch (Exception e)
        {
            Debug.LogError($"[SoundManager] Failed to pause sound: {e}");
        }
    }

    /// <summary>
    /// 恢复音效
    /// </summary>
    /// <param name="type">音效类型</param>
    /// <param name="clip">音效片段</param>
    public void ResumeSound(SoundType type, AudioClip clip)
    {
        if (!IsInitialized)
        {
            Debug.LogError("[SoundManager] Sound Manager has not been initialized");
            return;
        }

        try
        {
            _soundPoolManager.ResumeSound(new SoundItem(type, clip));
        }
        catch (Exception e)
        {
            Debug.LogError($"[SoundManager] Failed to resume sound: {e}");
        }
    }

    /// <summary>
    /// 是否正在播放音效
    /// </summary>
    /// <param name="type">音效类型</param>
    /// <param name="clip">音效片段</param>
    /// <returns>是否正在播放</returns>
    public bool IsPlaying(SoundType type, AudioClip clip)
    {
        if (!IsInitialized)
        {
            Debug.LogError("[SoundManager] Sound Manager has not been initialized");
            return false;
        }

        try
        {
            return _soundPoolManager.IsPlaying(new SoundItem(type, clip));
        }
        catch (Exception e)
        {
            Debug.LogError($"[SoundManager] Failed to check if sound is playing: {e}");
            return false;
        }
    }
    #endregion
}

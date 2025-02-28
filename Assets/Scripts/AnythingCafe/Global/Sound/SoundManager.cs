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
    private bool _isMuted;

    // 音量控制
    private float _globalVolumeFactor = 1f;
    private float _musicVolumeFactor = 1f;
    private float _sfxVolumeFactor = 1f;
    private float _ambientVolumeFactor = 1f;
    private float _uiVolumeFactor = 1f;
    private float _eroVolumeFactor = 1f;

    // 音频淡入淡出默认时间
    private const float DefaultFadeTime = 1.0f;

    public bool IsInitialized { get; set; }
    public bool IsMuted
    {
        get => _isMuted;
        set
        {
            _isMuted = value;
            SetMute(_isMuted);
        }
    }

    public void Init()
    {
        if (IsInitialized) return;

        // 初始化音频池
        _sourceParent ??= gameObject;
        _soundPoolManager = SoundPoolManager.Instance.Created(_sourceParent, _audioMixer);

        // 加载音频设置
        LoadOptions(OptionGroup.Audio);

        // 订阅设置变更事件
        OptionManager.Instance.OnOptionChanged += keyEnum => LoadOptions(keyEnum);

        IsInitialized = true;
    }

    private void LoadOptions(Enum notifier)
    {
        if (notifier is not OptionGroup and not OptionKey) return;

        switch (notifier)
        {
            case OptionGroup group when group == OptionGroup.Audio:
                LoadAllVolumeSettings();
                break;
            case OptionKey key:
                LoadSpecificVolumeSetting(key);
                break;
        }

        ApplyVolumeSettings();
    }

    private void LoadAllVolumeSettings()
    {
        _globalVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.GlobalVolume);
        _ambientVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.AmbientVolume);
        _eroVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.EroVolume);
        _musicVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.MusicVolume);
        _sfxVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.SFXVolume);
        _uiVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.UIVolume);
    }

    private void LoadSpecificVolumeSetting(OptionKey key)
    {
        switch (key)
        {
            case OptionKey.GlobalVolume:
                _globalVolumeFactor = OptionManager.Instance.GetValue<float>(key);
                break;
            case OptionKey.AmbientVolume:
                _ambientVolumeFactor = OptionManager.Instance.GetValue<float>(key);
                break;
            case OptionKey.EroVolume:
                _eroVolumeFactor = OptionManager.Instance.GetValue<float>(key);
                break;
            case OptionKey.MusicVolume:
                _musicVolumeFactor = OptionManager.Instance.GetValue<float>(key);
                break;
            case OptionKey.SFXVolume:
                _sfxVolumeFactor = OptionManager.Instance.GetValue<float>(key);
                break;
            case OptionKey.UIVolume:
                _uiVolumeFactor = OptionManager.Instance.GetValue<float>(key);
                break;
        }
    }

    private void ApplyVolumeSettings()
    {
        if (_audioMixer != null)
        {
            foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
            {
                var volumeFactor = GetVolumeFactorForType(type);
                _audioMixer.SetFloat($"{type}Volume", Mathf.Log10(volumeFactor * _globalVolumeFactor) * 20);
            }
        }
    }

    private float GetVolumeFactorForType(SoundType type)
    {
        return type switch
        {
            SoundType.Ambient => _ambientVolumeFactor,
            SoundType.Ero => _eroVolumeFactor,
            SoundType.Music => _musicVolumeFactor,
            SoundType.SFX => _sfxVolumeFactor,
            SoundType.UI => _uiVolumeFactor,
            _ => 1f
        };
    }

    private void SetMute(bool mute)
    {
        if (_audioMixer != null)
        {
            var volume = mute ? -80f : Mathf.Log10(_globalVolumeFactor) * 20;
            _audioMixer.SetFloat("MasterVolume", volume);
        }
    }

    #region 公共方法
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="type"> 音效类型 </param>
    /// <param name="clip"> 音效片段 </param>
    /// <param name="loop"> 是否循环播放 </param>
    /// <param name="volume"> 音量 </param>
    /// <param name="delay"> 延迟播放 </param>
    public void PlaySound(SoundType type, AudioClip clip, bool loop = false, float volume = 1.0f, ulong delay = 0)
    {
        if (!IsInitialized || clip == null) return;

        var finalVolume = volume * GetVolumeFactorForType(type) * _globalVolumeFactor;
        var soundItem = new SoundItem(type, clip, loop, finalVolume, delay);
        _soundPoolManager.PlaySound(soundItem);
    }

    /// <summary>
    /// 在指定位置播放音效
    /// </summary>
    /// <param name="type"> 音效类型 </param>
    /// <param name="clip"> 音效片段 </param>
    /// <param name="position"> 位置 </param>
    /// <param name="loop"> 是否循环播放 </param>
    /// <param name="volume"> 音量 </param>
    /// <param name="delay"> 延迟播放 </param>
    public void PlaySoundAtPosition(SoundType type, AudioClip clip, Vector3 position, bool loop = false, float volume = 1.0f, ulong delay = 0)
    {
        if (!IsInitialized || clip == null) return;

        var finalVolume = volume * GetVolumeFactorForType(type) * _globalVolumeFactor;
        var soundItem = new SoundItem(
            type, clip, loop, finalVolume, 1f, 1f, delay, null, position);
        _soundPoolManager.PlaySound(soundItem);
    }

    /// <summary>
    /// 停止音效
    /// </summary>
    /// <param name="type"> 音效类型 </param>
    /// <param name="clip"> 音效片段 </param>
    public void StopSound(SoundType type, AudioClip clip)
    {
        if (!IsInitialized) return;
        _soundPoolManager.StopSound(new SoundItem(type, clip));
    }

    /// <summary>
    /// 停止所有音效
    /// </summary>
    public void StopAllSounds()
    {
        if (!IsInitialized) return;
        _soundPoolManager.StopAllSounds();
    }

    /// <summary>
    /// 暂停音效
    /// </summary>
    /// <param name="type"> 音效类型 </param>
    /// <param name="clip"> 音效片段 </param>
    public void PauseSound(SoundType type, AudioClip clip)
    {
        if (!IsInitialized) return;
        _soundPoolManager.PauseSound(new SoundItem(type, clip));
    }

    /// <summary>
    /// 恢复音效
    /// </summary>
    /// <param name="type"> 音效类型 </param>
    /// <param name="clip"> 音效片段 </param>
    public void ResumeSound(SoundType type, AudioClip clip)
    {
        if (!IsInitialized) return;
        _soundPoolManager.ResumeSound(new SoundItem(type, clip));
    }

    /// <summary>
    /// 是否正在播放音效
    /// </summary>
    /// <param name="type"> 音效类型 </param>
    /// <param name="clip"> 音效片段 </param>
    /// <returns></returns>
    public bool IsPlaying(SoundType type, AudioClip clip)
    {
        return IsInitialized && _soundPoolManager.IsPlaying(new SoundItem(type, clip));
    }
    #endregion
}

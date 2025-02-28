using System;
using UnityEngine;

/// <summary>
/// 声音管理器
/// </summary>
[AddComponentMenu("FrameMonster/Sound/SoundManager")]
public class SoundManager : PersistentSingleton<SoundManager>, IInitializable
{
    private SoundPool _soundPool;

    private float _globalVolumeFactor = 1; // 全局音量
    private float _musicVolumeFactor = 1; // 背景音乐音量
    private float _sfxVolumeFactor = 1; // 音效音量
    private float _ambientVolumeFactor = 1; // 环境音量
    private float _uiVolumeFactor = 1; // UI音量
    private float _eroVolumeFactor = 1; // 色情声音

    [SerializeField] private GameObject _sourceParent; // 所有Source对应的GameObject的父物体

    public bool IsInitialized { get; set; }

    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        // 首次加载配置
        LoadOptions(OptionGroup.Audio);

        // 监听OptionManager的变化
        // OptionManager.Instance.OnGroupChanged += groupNotifier => LoadOptions(groupNotifier);
        OptionManager.Instance.OnOptionChanged += keyNotifier => LoadOptions(keyNotifier);

        // 初始化创建SoundPool
        if (_sourceParent == null) _sourceParent = gameObject;
        _soundPool ??= SoundPool.Instance.Created(_sourceParent);
    }

    private void LoadOptions(Enum notifier)
    {
        if (notifier is not OptionGroup and not OptionKey) return;

        switch (notifier)
        {
            case OptionGroup group when group == OptionGroup.Audio:
                _globalVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.GlobalVolume);
                _ambientVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.AmbientVolume);
                _eroVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.EroVolume);
                _musicVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.MusicVolume);
                _sfxVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.SFXVolume);
                _uiVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.UIVolume);
                break;
            case OptionKey key:
                switch (key)
                {
                    case OptionKey.GlobalVolume:
                        _globalVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.GlobalVolume);
                        break;
                    case OptionKey.AmbientVolume:
                        _ambientVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.AmbientVolume);
                        break;
                    case OptionKey.EroVolume:
                        _eroVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.EroVolume);
                        break;
                    case OptionKey.MusicVolume:
                        _musicVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.MusicVolume);
                        break;
                    case OptionKey.SFXVolume:
                        _sfxVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.SFXVolume);
                        break;
                    case OptionKey.UIVolume:
                        _uiVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.UIVolume);
                        break;
                }
                break;
        }
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
        // 音量乘数
        var volumeFactor = type switch
        {
            SoundType.Ambient => _ambientVolumeFactor,
            SoundType.Ero => _eroVolumeFactor,
            SoundType.Music => _musicVolumeFactor,
            SoundType.SFX => _sfxVolumeFactor,
            SoundType.UI => _uiVolumeFactor,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        } * _globalVolumeFactor;

        // 查找后包装一个
        var soundItem = new SoundItem(type, clip, loop, volume * volumeFactor, delay);
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

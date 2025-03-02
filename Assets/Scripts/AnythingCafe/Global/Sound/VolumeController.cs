using UnityEngine;
using UnityEngine.Audio;
using System;

/// <summary>
/// 音量控制器
/// </summary>
public class VolumeController
{
    private readonly AudioMixer _audioMixer;

    // 音量控制
    private float _globalVolumeFactor = 1f;
    private float _musicVolumeFactor = 1f;
    private float _sfxVolumeFactor = 1f;
    private float _ambientVolumeFactor = 1f;
    private float _uiVolumeFactor = 1f;
    private float _eroVolumeFactor = 1f;

    public VolumeController(AudioMixer audioMixer)
    {
        _audioMixer = audioMixer;
        LoadAllVolumeSettings();
    }

    public void LoadOptions(Enum optionEnum)
    {
        if (optionEnum is not OptionGroup and not OptionKey) return;

        switch (optionEnum)
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

    #region 私有方法
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
        if (_audioMixer == null) return;

        foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
        {
            var volumeFactor = GetVolumeFactorForType(type);
            // 音量范围为0~1，需要转换为-80~0
            _audioMixer.SetFloat($"{type}Volume", Mathf.Log10(volumeFactor * _globalVolumeFactor) * 20);
        }
    }
    #endregion

    #region 公有方法
    public float GetVolumeFactorForType(SoundType type)
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

    public float GetFinalVolume(SoundType type, float baseVolume)
    {
        return baseVolume * GetVolumeFactorForType(type) * _globalVolumeFactor;
    }
    #endregion
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OptionManager : Singleton<OptionManager>, IInitializable
{
    private readonly Dictionary<OptionKey, OptionValue> _options;

    // 分组变更事件
    public event Action<OptionGroup> OnGroupChanged;
    // 单个选项变更事件
    public event Action<OptionKey> OnOptionChanged;

    public OptionManager()
    {
        var defaults = OptionDefaults.Instance;
        _options = new Dictionary<OptionKey, OptionValue>
        {
            // 音频设置
            { OptionKey.GlobalVolume, new OptionValue<float>(defaults.defaultGlobalVolume, OptionValidator.ValidateVolume, OptionGroup.Audio) },
            { OptionKey.AmbientVolume, new OptionValue<float>(defaults.defaultAmbientVolume, OptionValidator.ValidateVolume, OptionGroup.Audio) },
            { OptionKey.EroVolume, new OptionValue<float>(defaults.defaultEroVolume, OptionValidator.ValidateVolume, OptionGroup.Audio) },
            { OptionKey.MusicVolume, new OptionValue<float>(defaults.defaultMusicVolume, OptionValidator.ValidateVolume, OptionGroup.Audio) },
            { OptionKey.SFXVolume, new OptionValue<float>(defaults.defaultSFXVolume, OptionValidator.ValidateVolume, OptionGroup.Audio) },
            { OptionKey.UIVolume, new OptionValue<float>(defaults.defaultUIVolume, OptionValidator.ValidateVolume, OptionGroup.Audio) },
            
            // 显示设置
            { OptionKey.ScreenMode, new OptionValue<ScreenMode>(defaults.defaultScreenMode, null, OptionGroup.Display) },
            { OptionKey.ResolutionWidth, new OptionValue<int>(defaults.defaultResolutionWidth, width => OptionValidator.ValidateResolution(width, GetValue<int>(OptionKey.ResolutionHeight)), OptionGroup.Display) },
            { OptionKey.ResolutionHeight, new OptionValue<int>(defaults.defaultResolutionHeight, height => OptionValidator.ValidateResolution(GetValue<int>(OptionKey.ResolutionWidth), height), OptionGroup.Display) },
            
            // 其他设置
            { OptionKey.Language, new OptionValue<string>(defaults.defaultLanguage, OptionValidator.ValidateLanguage, OptionGroup.Other) }
        };
    }

    public bool IsInitialized { get; set; }

    public void Init()
    {
        if (IsInitialized) return;
        LoadAll();
        IsInitialized = true;
    }

    public T GetValue<T>(OptionKey key)
    {
        if (_options.TryGetValue(key, out var value))
            return (T)value.GetValue();
        throw new KeyNotFoundException($"Option key {key} not found");
    }

    public void SetValue<T>(OptionKey key, T value)
    {
        if (!_options.TryGetValue(key, out var optionValue))
            throw new KeyNotFoundException($"Option key {key} not found");

        if (!optionValue.Validate(value))
            throw new ArgumentException($"Invalid value for {key}: {value}");

        var oldGroup = optionValue.Group;
        optionValue.SetValue(value);

        OnOptionChanged?.Invoke(key);
        OnGroupChanged?.Invoke(oldGroup);
    }

    public void ResetGroup(OptionGroup group)
    {
        var groupKeys = _options.Where(kvp => kvp.Value.Group == group)
                               .Select(kvp => kvp.Key)
                               .ToList();

        foreach (var key in groupKeys)
            ResetToDefault(key);

        OnGroupChanged?.Invoke(group);
    }

    public void ResetToDefault(OptionKey key)
    {
        if (_options.TryGetValue(key, out var value))
        {
            value.SetValue(value.GetDefaultValue());
            OnOptionChanged?.Invoke(key);
            OnGroupChanged?.Invoke(value.Group);
        }
    }

    public void ResetAll()
    {
        foreach (var group in Enum.GetValues(typeof(OptionGroup)).Cast<OptionGroup>())
            ResetGroup(group);
    }

    public void Save()
    {
        var settings = new ES3Settings(
            SaveFileConstants.Options,
            ES3.EncryptionType.AES,
            "AnythingCafe"
        );

        foreach (var kvp in _options)
        {
            ES3.Save(kvp.Key.ToString(), kvp.Value.GetValue(), settings);
        }
    }

    private void LoadAll()
    {
        var settings = new ES3Settings(
            SaveFileConstants.Options,
            ES3.EncryptionType.AES,
            "AnythingCafe"
        );

        foreach (var kvp in _options)
        {
            try
            {
                var value = ES3.Load(kvp.Key.ToString(), kvp.Value.GetDefaultValue(), settings);
                kvp.Value.SetValue(value);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[OptionManager] Failed to load option {kvp.Key}: {e.Message}");
                kvp.Value.SetValue(kvp.Value.GetDefaultValue());
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OptionManager : Singleton<OptionManager>, IInitializable
{
    private const string Password = "AnythingCafe";
    private const string SaveFile = SaveFileConstants.Options;

    private readonly Dictionary<OptionKey, OptionValue> _options;

    public event Action<OptionGroup> OnGroupChanged;    // 分组变更事件
    public event Action<OptionKey> OnOptionChanged;    // 单个选项变更事件

    public OptionManager()
    {
        // 加载默认值
        var defaults = OptionDefaults.Instance;
        _options = new Dictionary<OptionKey, OptionValue>
        {
            // 音频设置
            { OptionKey.GlobalVolume, new OptionValue<float>(defaults.DefaultGlobalVolume, OptionValidator.ValidateVolume, OptionGroup.Audio) },
            { OptionKey.AmbientVolume, new OptionValue<float>(defaults.DefaultAmbientVolume, OptionValidator.ValidateVolume, OptionGroup.Audio) },
            { OptionKey.EroVolume, new OptionValue<float>(defaults.DefaultEroVolume, OptionValidator.ValidateVolume, OptionGroup.Audio) },
            { OptionKey.MusicVolume, new OptionValue<float>(defaults.DefaultMusicVolume, OptionValidator.ValidateVolume, OptionGroup.Audio) },
            { OptionKey.SFXVolume, new OptionValue<float>(defaults.DefaultSFXVolume, OptionValidator.ValidateVolume, OptionGroup.Audio) },
            { OptionKey.UIVolume, new OptionValue<float>(defaults.DefaultUIVolume, OptionValidator.ValidateVolume, OptionGroup.Audio) },
            
            // 显示设置
            { OptionKey.ScreenMode, new OptionValue<ScreenMode>(defaults.DefaultScreenMode, null, OptionGroup.Display) },
            { OptionKey.ResolutionWidth, new OptionValue<int>(defaults.DefaultResolutionWidth, width => OptionValidator.ValidateResolution(width, GetValue<int>(OptionKey.ResolutionHeight)), OptionGroup.Display) },
            { OptionKey.ResolutionHeight, new OptionValue<int>(defaults.DefaultResolutionHeight, height => OptionValidator.ValidateResolution(GetValue<int>(OptionKey.ResolutionWidth), height), OptionGroup.Display) },
            
            // 其他设置
            { OptionKey.Language, new OptionValue<string>(defaults.DefaultLanguage, OptionValidator.ValidateLanguage) }
        };
    }

    public bool IsInitialized { get; set; }
    public void Init()
    {
        if (IsInitialized) return;

        // 首次加载Option数据
        LoadAll();

        IsInitialized = true;
    }

    /// <summary>
    /// 获取选项值
    /// </summary>
    /// <typeparam name="T"> 选项值类型 </typeparam>
    /// <param name="key"> 选项 </param>
    /// <returns> 选项值 </returns>
    /// <exception cref="KeyNotFoundException"> 选项不存在 </exception>
    public T GetValue<T>(OptionKey key)
    {
        if (_options.TryGetValue(key, out var value))
            return (T)value.GetValue();
        throw new KeyNotFoundException($"[OptionManager] Option key {key} not found");
    }

    /// <summary>
    /// 设置选项值
    /// </summary>
    /// <typeparam name="T"> 选项值类型 </typeparam>
    /// <param name="key"> 选项 </param>
    /// <param name="value"> 选项值 </param>
    /// <exception cref="KeyNotFoundException"> 选项不存在 </exception>
    /// <exception cref="ArgumentException"> 选项值无效 </exception>
    public void SetValue<T>(OptionKey key, T value)
    {
        if (!_options.TryGetValue(key, out var optionValue))
            throw new KeyNotFoundException($"[OptionManager] Option key {key} not found");

        if (!optionValue.Validate(value))
            throw new ArgumentException($"[OptionManager] Invalid value for {key}: {value}");

        var oldGroup = optionValue.Group;
        optionValue.SetValue(value);

        OnOptionChanged?.Invoke(key);
        OnGroupChanged?.Invoke(oldGroup);
    }

    /// <summary>
    /// 重置指定分组的所有选项到默认值
    /// </summary>
    /// <param name="group"> 分组 </param>
    public void ResetGroup(OptionGroup group)
    {
        var groupKeys = _options.Where(kvp => kvp.Value.Group == group)
                               .Select(kvp => kvp.Key)
                               .ToList();

        foreach (var key in groupKeys)
            ResetToDefault(key);

        OnGroupChanged?.Invoke(group);
    }

    /// <summary>
    /// 重置指定选项到默认值
    /// </summary>
    /// <param name="key"> 选项 </param>
    public void ResetToDefault(OptionKey key)
    {
        if (_options.TryGetValue(key, out var value))
        {
            value.SetValue(value.GetDefaultValue());
            OnOptionChanged?.Invoke(key);
            OnGroupChanged?.Invoke(value.Group);
        }
    }

    /// <summary>
    /// 重置所有选项到默认值
    /// </summary>
    public void ResetAll()
    {
        foreach (var group in Enum.GetValues(typeof(OptionGroup)).Cast<OptionGroup>())
            ResetGroup(group);
    }

    /// <summary>
    /// 保存所有选项值,推荐只在退出游戏时调用
    /// </summary>
    public void Save()
    {
        var settings = new ES3Settings(
            SaveFile,
            ES3.EncryptionType.AES,
            Password
        );

        foreach (var kvp in _options)
        {
            ES3.Save(kvp.Key.ToString(), kvp.Value.GetValue(), settings);
        }
    }

    /// <summary>
    /// 清除所有观察者
    /// </summary>
    public void ClearObservers()
    {
        OnOptionChanged = null;
        OnGroupChanged = null;
    }

    private void LoadAll()
    {
        var settings = new ES3Settings(
            SaveFile,
            ES3.EncryptionType.AES,
            Password
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
using System;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : Singleton<OptionManager>, IInitializable
{
    private readonly Dictionary<OptionKey, OptionValue> _options = new()
    {
        { OptionKey.GlobalVolume, new OptionValue<float>(1.0f) },
        { OptionKey.AmbientVolume, new OptionValue<float>(1.0f) },
        { OptionKey.EroVolume, new OptionValue<float>(1.0f) },
        { OptionKey.MusicVolume, new OptionValue<float>(1.0f) },
        { OptionKey.SFXVolume, new OptionValue<float>(1.0f) },
        { OptionKey.UIVolume, new OptionValue<float>(1.0f) },
        { OptionKey.ScreenMode, new OptionValue<ScreenMode>(ScreenMode.Windowed) },
        { OptionKey.ResolutionWidth, new OptionValue<int>(1920) },
        { OptionKey.ResolutionHeight, new OptionValue<int>(1080) },
        { OptionKey.Language, new OptionValue<string>("en") }
    };

    public event Action<OptionKey> OnOptionChanged;

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
        {
            return (T)value.GetValue();
        }
        throw new KeyNotFoundException($"Option key {key} not found");
    }

    public void SetValue<T>(OptionKey key, T value)
    {
        if (_options.TryGetValue(key, out var optionValue))
        {
            optionValue.SetValue(value);
            OnOptionChanged?.Invoke(key);
        }
        else
        {
            throw new KeyNotFoundException($"Option key {key} not found");
        }
    }

    public void ResetToDefault(OptionKey key)
    {
        if (_options.TryGetValue(key, out var value))
        {
            value.SetValue(value.GetDefaultValue());
            OnOptionChanged?.Invoke(key);
        }
    }

    public void ResetAll()
    {
        foreach (var key in _options.Keys)
        {
            ResetToDefault(key);
        }
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
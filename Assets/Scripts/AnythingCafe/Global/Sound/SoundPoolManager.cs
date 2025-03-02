using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 音频源池管理器
/// </summary>
public class SoundPoolManager : IAudioSourcePool
{
    private const int DefaultPoolSize = 10;
    private const int MaxSourcesPerType = 50;

    private readonly bool _created = false;
    private readonly GameObject _sourceParent;

    // 对象池
    private readonly Dictionary<SoundType, HashSet<AudioSource>> _idleSources = new();
    private readonly Dictionary<SoundType, HashSet<AudioSource>> _busySources = new();
    private readonly Dictionary<SoundType, Stack<SoundItem>> _soundItemPool = new();

    // 音频混音器组
    private readonly Dictionary<SoundType, AudioMixerGroup> _mixerGroups = new();

    /// <summary>
    /// 初始化音频源池
    /// </summary>
    /// <param name="sourceParent">音频源池GameObject的父对象</param>
    /// <param name="audioMixer">音频混音器组</param>
    public SoundPoolManager(GameObject sourceParent, AudioMixer audioMixer = null)
    {
        if (_created)
        {
            Debug.LogWarning("[SoundPoolManager] 音频源池已经初始化过了");
            return;
        }

        try
        {
            _created = true;
            _sourceParent = sourceParent;

            foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
            {
                _idleSources[type] = new HashSet<AudioSource>();
                _busySources[type] = new HashSet<AudioSource>();
                _soundItemPool[type] = new Stack<SoundItem>();

                // 如果提供了AudioMixer，设置混音器组
                if (audioMixer != null)
                {
                    var group = audioMixer.FindMatchingGroups(type.ToString()).FirstOrDefault();
                    if (group != null)
                    {
                        _mixerGroups[type] = group;
                    }
                }
            }

            ExpandSourcePool();
        }
        catch (Exception e)
        {
            Debug.LogError($"[SoundPoolManager] 初始化失败: {e}");
            _created = false;
        }
    }

    #region IAudioSourcePool 实现
    public AudioSource GetSource(SoundType soundType)
    {
        RecycleBusySources(soundType);

        if (!_idleSources[soundType].Any())
        {
            // 检查是否达到最大限制
            if (_busySources[soundType].Count >= MaxSourcesPerType)
            {
                Debug.LogWarning($"[SoundPoolManager] 达到音频源上限 ({MaxSourcesPerType}) for {soundType}");
                return _busySources[soundType].First();
            }

            ExpandSourcePool(soundType);
        }

        var source = _idleSources[soundType].First();
        _idleSources[soundType].Remove(source);
        _busySources[soundType].Add(source);
        return source;
    }

    public void RecycleSource(AudioSource source, SoundType soundType)
    {
        if (source == null) return;

        if (_busySources[soundType].Remove(source))
        {
            ResetSource(source);
            _idleSources[soundType].Add(source);
        }
    }

    public AudioMixerGroup GetMixerGroup(SoundType soundType)
    {
        return _mixerGroups.GetValueOrDefault(soundType);
    }

    public void RecycleAllSources(SoundType soundType)
    {
        var sourcesToRecycle = _busySources[soundType].ToList();
        foreach (var source in sourcesToRecycle)
        {
            RecycleSource(source, soundType);
        }
    }

    public void RecycleAllSources()
    {
        foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
        {
            RecycleAllSources(type);
        }
    }
    #endregion

    #region SoundItem 对象池
    public SoundItem GetSoundItem(
        SoundType soundType,
        AudioClip audioClip,
        bool loop = false,
        float volume = 1.0f,
        float pitch = 1.0f,
        float spatialBlend = 0f,
        ulong delay = 0,
        AudioMixerGroup outputAudioMixerGroup = null,
        Vector3? position = null)
    {
        if (!_soundItemPool[soundType].TryPop(out var item))
        {
            item = new SoundItem(soundType, audioClip, loop, volume, pitch, spatialBlend, delay, outputAudioMixerGroup, position);
        }
        else
        {
            item.SoundType = soundType;
            item.AudioClip = audioClip;
            item.Loop = loop;
            item.Volume = volume;
            item.Pitch = pitch;
            item.SpatialBlend = spatialBlend;
            item.Delay = delay;
            item.OutputAudioMixerGroup = outputAudioMixerGroup;
            item.Position = position;
        }
        return item;
    }

    public void RecycleSoundItem(SoundItem item)
    {
        if (item == null) return;
        _soundItemPool[item.SoundType].Push(item);
    }
    #endregion

    #region 私有方法
    private void RecycleBusySources(SoundType soundType)
    {
        var sourcesToRecycle = _busySources[soundType]
            .Where(source => !source.isPlaying)
            .ToList();

        foreach (var source in sourcesToRecycle)
        {
            RecycleSource(source, soundType);
        }
    }

    private void ResetSource(AudioSource source)
    {
        source.clip = null;
        source.loop = false;
        source.volume = 1f;
        source.pitch = 1f;
        source.spatialBlend = 0f;
        source.outputAudioMixerGroup = null;
        source.transform.localPosition = Vector3.zero;
    }

    private void ExpandSourcePool(SoundType? specificType = null)
    {
        try
        {
            if (specificType.HasValue)
            {
                CreateSources(specificType.Value);
            }
            else
            {
                foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
                {
                    CreateSources(type);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SoundPoolManager] 扩展音频源池失败: {e}");
        }
    }

    private void CreateSources(SoundType type)
    {
        for (var i = 0; i < DefaultPoolSize; i++)
        {
            var sourceObj = new GameObject($"AudioSource_{type}_{_idleSources[type].Count}");
            sourceObj.transform.SetParent(_sourceParent.transform);

            var source = sourceObj.AddComponent<AudioSource>();
            source.playOnAwake = false;

            if (_mixerGroups.TryGetValue(type, out var mixerGroup))
            {
                source.outputAudioMixerGroup = mixerGroup;
            }

            _idleSources[type].Add(source);
        }
    }
    #endregion
}

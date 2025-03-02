using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 音频源池管理器
/// </summary>
public class SoundPoolManager
{
    private const int DefaultPoolSize = 10;
    private const int MaxSourcesPerType = 50;

    private readonly bool _created = false;
    private readonly GameObject _sourceParent;

    // 对象池
    // 包括Source池(播放音频)和Item池(存储音频信息)
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
    /// <returns>返回SoundPool实例</returns>
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

            // 初始化所有池
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
#if VERBOSE_LOG
                        Debug.Log($"[SoundPoolManager] 设置{type}的AudioMixerGroup为{group.name}");
#endif
                        _mixerGroups[type] = group;
                    }
                }
            }

            // 初始化音频源
            ExpandSourcePool();
        }
        catch (Exception e)
        {
            Debug.LogError($"[SoundPoolManager] 初始化失败: {e}");
            _created = false;
        }
    }

    #region 事件系统
    private readonly SoundEventSystem _eventSystem = new();

    public event Action<AudioSource> OnSoundPlay
    {
        add => _eventSystem.OnSoundPlay += value;
        remove => _eventSystem.OnSoundPlay -= value;
    }

    public event Action<AudioSource> OnSoundStop
    {
        add => _eventSystem.OnSoundStop += value;
        remove => _eventSystem.OnSoundStop -= value;
    }

    public event Action<AudioSource> OnSoundPause
    {
        add => _eventSystem.OnSoundPause += value;
        remove => _eventSystem.OnSoundPause -= value;
    }

    public event Action<AudioSource> OnSoundResume
    {
        add => _eventSystem.OnSoundResume += value;
        remove => _eventSystem.OnSoundResume -= value;
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 获取一个音频源
    /// </summary>
    private AudioSource GetSource(SoundType soundType)
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

    /// <summary>
    /// 回收闲置音频源
    /// </summary>
    private void RecycleBusySources(SoundType soundType)
    {
        var sourcesToRecycle = _busySources[soundType].Where(source => !source.isPlaying).ToList();
        foreach (var source in sourcesToRecycle)
        {
            _busySources[soundType].Remove(source);
            ResetSource(source);
            _idleSources[soundType].Add(source);
            _eventSystem.TriggerSoundStop(source);
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

    /// <summary>
    /// 扩展音频源池
    /// </summary>
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

    /// <summary>
    /// 创建音频源
    /// </summary>
    /// <param name="type"> 音频类型 </param>
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

    /// <summary>
    /// 从对象池获取或创建SoundItem
    /// </summary>
    private SoundItem GetOrCreateSoundItem(
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

    /// <summary>
    /// 回收SoundItem到对象池
    /// </summary>
    private void RecycleSoundItem(SoundItem item)
    {
        if (item == null) return;
        _soundItemPool[item.SoundType].Push(item);
    }
    #endregion

    #region 公共方法
    /// <summary>
    /// 播放音频
    /// </summary>
    public void PlaySound(SoundItem sound)
    {
        if (!_created)
        {
            Debug.LogError("[SoundPoolManager] 音频源池未初始化");
            return;
        }

        if (sound?.AudioClip == null)
        {
            Debug.LogWarning("[SoundPoolManager] 尝试播放空的AudioClip");
            return;
        }

        try
        {
            var source = GetSource(sound.SoundType);
            sound.OutputAudioMixerGroup = source.outputAudioMixerGroup;
            sound.ApplyToSource(source);

            if (sound.Delay > 0)
            {
                source.PlayDelayed(sound.Delay / 1000f);
            }
            else
            {
                source.Play();
            }

            _eventSystem.TriggerSoundPlay(source);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SoundPoolManager] 播放音频失败: {e}");
        }
    }

    /// <summary>
    /// 停止音频
    /// </summary>
    public void StopSound(SoundItem sound)
    {
        if (!_created || sound?.AudioClip == null) return;

        foreach (var source in _busySources[sound.SoundType].Where(s => s.clip == sound.AudioClip))
        {
            source.Stop();
            _eventSystem.TriggerSoundStop(source);
        }
    }

    /// <summary>
    /// 停止所有音频
    /// </summary>
    public void StopAllSounds()
    {
        if (!_created) return;

        foreach (var source in _busySources.Values.SelectMany(sources => sources))
        {
            source.Stop();
            _eventSystem.TriggerSoundStop(source);
        }
    }

    /// <summary>
    /// 暂停音频
    /// </summary>
    public void PauseSound(SoundItem sound)
    {
        if (!_created || sound?.AudioClip == null) return;

        foreach (var source in _busySources[sound.SoundType].Where(s => s.clip == sound.AudioClip))
        {
            source.Pause();
            _eventSystem.TriggerSoundPause(source);
        }
    }

    /// <summary>
    /// 恢复音频
    /// </summary>
    public void ResumeSound(SoundItem sound)
    {
        if (!_created || sound?.AudioClip == null) return;

        foreach (var source in _busySources[sound.SoundType].Where(s => s.clip == sound.AudioClip))
        {
            source.UnPause();
            _eventSystem.TriggerSoundResume(source);
        }
    }

    /// <summary>
    /// 检查音频是否正在播放
    /// </summary>
    public bool IsPlaying(SoundItem sound)
    {
        return _created && sound?.AudioClip != null &&
               _busySources[sound.SoundType].Any(s => s.clip == sound.AudioClip && s.isPlaying);
    }
    #endregion
}

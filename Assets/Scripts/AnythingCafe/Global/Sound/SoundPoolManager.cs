using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
///  音频源池
/// </summary>
public class SoundPoolManager : Singleton<SoundPoolManager>
{
    private const int DefaultPoolSize = 10;
    private const int MaxSourcesPerType = 50;

    private bool _created = false;

    private GameObject _sourceParent;

    // 音频源池
    private readonly Dictionary<SoundType, Stack<AudioSource>> _idleSources = new();
    private readonly Dictionary<SoundType, Stack<AudioSource>> _busySources = new();

    // 音频混音器组
    private readonly Dictionary<SoundType, AudioMixerGroup> _mixerGroups = new();

    public event Action<AudioSource> OnSoundPlay;
    public event Action<AudioSource> OnSoundStop;
    public event Action<AudioSource> OnSoundPause;
    public event Action<AudioSource> OnSoundResume;

    /// <summary>
    /// 初始化音频源池
    /// </summary>
    /// <param name="sourceParent">
    /// 音频源池GameObject的父对象，
    /// 如果音频源池GameObject在场景中，
    /// 默认应为SoundManager对应的GameObject
    /// </param>
    /// <param name="audioMixer"> 音频混音器组 </param>
    /// <returns> 返回SoundPool实例 </returns>
    public SoundPoolManager Created(GameObject sourceParent, AudioMixer audioMixer = null)
    {
        if (_created) return Instance;

        _created = true;
        _sourceParent = sourceParent;

        // 初始化音频源池
        foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
        {
            _idleSources[type] = new Stack<AudioSource>();
            _busySources[type] = new Stack<AudioSource>();

            // 如果提供了AudioMixer，设置混音器组
            if (audioMixer != null)
            {
                // 替换以下代码行
                // audioMixer.GetGroup(type.ToString(), out var group);
                // _mixerGroups[type] = group;

                // 使用以下代码
                var group = audioMixer.FindMatchingGroups(type.ToString()).FirstOrDefault();
                if (group != null)
                {
                    _mixerGroups[type] = group;
                }
            }
        }

        // 初始化音频源
        ExpandSourcePool();
        return Instance;
    }

    #region 私有方法, 内部调用
    /// <summary>
    /// 获取一个音频源池中的音频源
    /// </summary>
    /// <returns> 返回一个音频源池中的音频源 </returns>
    private AudioSource GetSource(SoundType soundType)
    {
        RecycleBusySources(soundType);

        if (_idleSources[soundType].Count == 0)
        {
            // 检查是否达到最大限制
            if (_busySources[soundType].Count >= MaxSourcesPerType)
            {
                Debug.LogWarning($"[SoundPoolManager] 达到音频源上限 ({MaxSourcesPerType}) for {soundType}");
                return _busySources[soundType].Peek();
            }
            ExpandSourcePool(soundType);
        }

        var source = _idleSources[soundType].Pop();
        _busySources[soundType].Push(source);
        return source;
    }

    /// <summary>
    /// 回收闲置音频源
    /// </summary>
    private void RecycleBusySources(SoundType soundType)
    {
        if (!_busySources.TryGetValue(soundType, out var busySource)) return;

        var sourcesToRecycle = busySource.Where(source => !source.isPlaying).ToList();
        foreach (var source in sourcesToRecycle)
        {
            _busySources[soundType].Pop();
            ResetSource(source);
            _idleSources[soundType].Push(source);
            OnSoundStop?.Invoke(source);
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
    /// <param name="specificType"> 特定类型 </param>
    private void ExpandSourcePool(SoundType? specificType = null)
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

            _idleSources[type].Push(source);
        }
    }
    #endregion

    #region 公共方法, 外部调用
    /// <summary>
    /// 播放音频
    /// </summary>
    /// <param name="sound"> 音频 </param>
    public void PlaySound(SoundItem sound)
    {
        if (sound.AudioClip == null)
        {
            Debug.LogWarning("[SoundPoolManager] 尝试播放空的AudioClip");
            return;
        }

        var source = GetSource(sound.SoundType);
        sound.ApplyToSource(source);

        if (sound.Delay > 0)
        {
            // 按s计时
            source.PlayDelayed(sound.Delay / 1000f);
        }
        else
        {
            source.Play();
        }

        OnSoundPlay?.Invoke(source);
    }

    /// <summary>
    /// 停止音频
    /// </summary>
    /// <param name="sound"></param>
    public void StopSound(SoundItem sound)
    {
        if (!_busySources.TryGetValue(sound.SoundType, out var busySource)) return;

        foreach (var source in busySource.Where(s => s.clip == sound.AudioClip))
        {
            source.Stop();
            OnSoundStop?.Invoke(source);
        }
    }
    public void StopAllSounds()
    {
        foreach (var source in _busySources.Values.SelectMany(sources => sources))
        {
            source.Stop();
            OnSoundStop?.Invoke(source);
        }
    }

    /// <summary>
    /// 暂停音频
    /// </summary>
    /// <param name="sound"></param>
    public void PauseSound(SoundItem sound)
    {
        if (!_busySources.TryGetValue(sound.SoundType, out var busySource)) return;

        foreach (var source in busySource.Where(s => s.clip == sound.AudioClip))
        {
            source.Pause();
            OnSoundPause?.Invoke(source);
        }
    }

    /// <summary>
    /// 恢复音频
    /// </summary>
    /// <param name="sound"></param>
    public void ResumeSound(SoundItem sound)
    {
        if (!_busySources.TryGetValue(sound.SoundType, out var busySource)) return;

        foreach (var source in busySource.Where(s => s.clip == sound.AudioClip))
        {
            source.UnPause();
            OnSoundResume?.Invoke(source);
        }
    }

    /// <summary>
    /// 设置音频音量
    /// </summary>
    /// <param name="soundType"> 音频类型 </param>
    /// <param name="volume"> 音量 </param>
    public void SetVolume(SoundType soundType, float volume)
    {
        if (_mixerGroups.TryGetValue(soundType, out var mixerGroup))
        {
            mixerGroup.audioMixer.SetFloat($"{soundType}Volume", Mathf.Log10(volume) * 20);
        }
        else
        {
            foreach (var source in _busySources[soundType].Concat(_idleSources[soundType]))
            {
                source.volume = volume;
            }
        }
    }

    /// <summary>
    /// 设置音频音调
    /// </summary>
    /// <param name="soundType"> 音频类型 </param>
    /// <param name="pitch"> 音调 </param>
    public void SetPitch(SoundType soundType, float pitch)
    {
        foreach (var source in _busySources[soundType].Concat(_idleSources[soundType]))
        {
            source.pitch = pitch;
        }
    }

    /// <summary>
    /// 检查音频是否正在播放
    /// </summary>
    /// <param name="sound"></param>
    /// <returns></returns>
    public bool IsPlaying(SoundItem sound)
    {
        return _busySources[sound.SoundType]
            .Any(s => s.clip == sound.AudioClip && s.isPlaying);
    }
    #endregion
}

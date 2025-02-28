using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
///  音频源对象池
/// </summary>
public class SoundPool : Singleton<SoundPool>
{
    private bool _created = false;

    private GameObject _sourceParent;
    private Dictionary<SoundType, Stack<AudioSource>> _idleSources; // 空闲的音频源对象
    private Dictionary<SoundType, Stack<AudioSource>> _busySources; // 正在播放的音频源对象

    public event Action<AudioSource> OnSoundPlay;// 音效播放事件
    public event Action<AudioSource> OnSoundStop; // 音效停止事件

    /// <summary>
    /// 创建音频源对象池
    /// </summary>
    /// <param name="sourceParent">
    /// 音频源对象池GameObject的父物体，
    /// 决定音频源对象池GameObject在场景中的位置,
    /// 默认设置为SoundManager组件对应的GameObject
    /// </param>
    /// <returns> 返回SoundPool实例 </returns>
    public SoundPool Created([NotNull] GameObject sourceParent)
    {
#if VERBOSE_LOG
        Debug.Log($"[SoundPool] Initializing sound pool with source parent: {sourceParent.name}");
#endif
        if (_created) return Instance;

        _created = true;
        _sourceParent = sourceParent;

        // 初始化空闲和正在播放的音频源对象池
        _idleSources = Enum.GetValues(typeof(SoundType))
            .Cast<SoundType>()
            .ToDictionary(type => type, _ => new Stack<AudioSource>());
        _busySources = Enum.GetValues(typeof(SoundType))
            .Cast<SoundType>()
            .ToDictionary(type => type, _ => new Stack<AudioSource>());

        ExpandSourceList();

        return Instance;
    }

    #region 私有方法, 一般用于管理对象池逻辑
    /// <summary>
    /// 获取一个空闲的音频源对象
    /// </summary>
    /// <returns> 返回一个空闲的音频源对象 </returns>
    private AudioSource GetSource(SoundType soundType)
    {
        // 回收空闲的音频源对象
        RecycleBusySources(soundType);

        // 如果回收后空闲的音频源对象池仍然为空，则扩充音频源对象池
        if (_idleSources[soundType].Count == 0)
        {
            ExpandSourceList(soundType);
        }

        var source = _idleSources[soundType].Pop();
        _busySources[soundType].Push(source);
        return source;
    }

    /// <summary>
    /// 回收空闲的BusySource
    /// </summary>
    private void RecycleBusySources(SoundType soundType)
    {
        var sourcesToRecycle = _busySources[soundType].Where(source => !source.isPlaying).ToList();
        foreach (var source in sourcesToRecycle)
        {
            _busySources[soundType].Pop();
            _idleSources[soundType].Push(source);
            OnSoundStop?.Invoke(source);
        }
    }

    /// <summary>
    /// 扩充某一类型的音频源对象池
    /// </summary>
    /// <param name="soundType"> 音频类型 </param>
    /// <param name="count"> 扩充数量，默认为10 </param>
    private void ExpandSourceList(SoundType soundType, int count = 10)
    {
        for (var i = 0; i < count; i++)
        {
            var newSourceObject = new GameObject(
                "AudioSource" + $"{soundType.ToString()}{_idleSources[soundType].Count}",
                new[]
                {
                    typeof(AudioSource)
                });
#if VERBOSE_LOG
            Debug.Log($"[SoundPool] Created new audio source object: {newSourceObject.name}");
#endif
            newSourceObject.transform.SetParent(_sourceParent.transform);
            _idleSources[soundType].Push(newSourceObject.GetComponent<AudioSource>());
        }
    }

    /// <summary>
    /// 扩充所有类型的音频源对象池
    /// </summary>
    /// <param name="count"> 扩充数量，默认为10 </param>
    private void ExpandSourceList(int count = 10)
    {
        foreach (var soundType in Enum.GetValues(typeof(SoundType)).Cast<SoundType>())
        {
            ExpandSourceList(soundType, count);
        }
    }
    #endregion

    #region 公共方法, 用于外部调用
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="sound"> 音效 </param>
    public void PlaySound(SoundItem sound)
    {
#if VERBOSE_LOG
        Debug.Log($"[SoundPool] Playing sound: {sound.AudioClip.name}");
#endif
        var source = GetSource(sound.SoundType);
#if VERBOSE_LOG
        Debug.Log($"Get source: {source.name}");
#endif
        source.clip = sound.AudioClip;
        source.loop = sound.Loop;
        source.volume = sound.Volume;
        source.Play(sound.Delay);
        OnSoundPlay?.Invoke(source);
    }

    /// <summary>
    /// 停止音效
    /// </summary>
    /// <param name="sound"></param>
    public void StopSound(SoundItem sound)
    {
#if VERBOSE_LOG
        Debug.Log($"[SoundPool] Stopping sound: {sound.AudioClip.name}");
#endif
        foreach (var source in _busySources[sound.SoundType].Where(source => source.clip == sound.AudioClip))
        {
            source.Stop();
            OnSoundStop?.Invoke(source);
        }
    }
    public void StopAllSounds()
    {
        foreach (var soundType in Enum.GetValues(typeof(SoundType)).Cast<SoundType>())
        {
            foreach (var source in _busySources[soundType])
            {
                source.Stop();
                OnSoundStop?.Invoke(source);
            }
        }
    }

    /// <summary>
    /// 暂停音效
    /// </summary>
    /// <param name="sound"></param>
    public void PauseSound(SoundItem sound)
    {
#if VERBOSE_LOG
        Debug.Log($"[SoundPool] Pausing sound: {sound.AudioClip.name}");
#endif
        foreach (var source in _busySources[sound.SoundType].Where(source => source.clip == sound.AudioClip))
        {
            source.Pause();
        }
    }

    /// <summary>
    /// 恢复音效
    /// </summary>
    /// <param name="sound"></param>
    public void ResumeSound(SoundItem sound)
    {
#if VERBOSE_LOG
        Debug.Log($"[SoundPool] Resuming sound: {sound.AudioClip.name}");
#endif
        foreach (var source in _busySources[sound.SoundType].Where(source => source.clip == sound.AudioClip))
        {
            source.UnPause();
        }
    }

    /// <summary>
    /// 设置音量
    /// </summary>
    /// <param name="volume"> 音量 </param>
    public void SetVolume(float volume)
    {
#if VERBOSE_LOG
        Debug.Log($"[SoundPool] Setting volume to: {volume}");  
#endif
        foreach (var soundType in Enum.GetValues(typeof(SoundType)).Cast<SoundType>())
        {
            foreach (var source in _idleSources[soundType])
            {
                source.volume = volume;
            }
            foreach (var source in _busySources[soundType])
            {
                source.volume = volume;
            }
        }
    }

    /// <summary>
    /// 设置音效类型音量
    /// </summary>
    /// <param name="soundType"> 音效类型 </param>
    /// <param name="volume"> 音量 </param>
    public void SetVolume(SoundType soundType, float volume)
    {
#if VERBOSE_LOG
        Debug.Log($"[SoundPool] Setting {soundType} volume to: {volume}");
#endif
        foreach (var source in _idleSources[soundType])
        {
            source.volume = volume;
        }
        foreach (var source in _busySources[soundType])
        {
            source.volume = volume;
        }
    }

    /// <summary>
    /// 设置音效播放速度
    /// </summary>
    /// <param name="pitch"> 播放速度 </param>
    public void SetPitch(float pitch)
    {
#if VERBOSE_LOG
        Debug.Log($"[SoundPool] Setting pitch to: {pitch}");
#endif
        foreach (var soundType in Enum.GetValues(typeof(SoundType)).Cast<SoundType>())
        {
            foreach (var source in _idleSources[soundType])
            {
                source.pitch = pitch;
            }
            foreach (var source in _busySources[soundType])
            {
                source.pitch = pitch;
            }
        }
    }

    /// <summary>
    /// 设置音效类型播放速度
    /// </summary>
    /// <param name="soundType"> 音效类型 </param>
    /// <param name="pitch"> 播放速度 </param>
    public void SetPitch(SoundType soundType, float pitch)
    {
#if VERBOSE_LOG
        Debug.Log($"[SoundPool] Setting {soundType} pitch to: {pitch}");    
#endif
        foreach (var source in _idleSources[soundType])
        {
            source.pitch = pitch;
        }
        foreach (var source in _busySources[soundType])
        {
            source.pitch = pitch;
        }
    }
    #endregion
}

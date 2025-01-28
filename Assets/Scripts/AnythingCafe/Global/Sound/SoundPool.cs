using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public class SoundPool : Singleton<SoundPool>
{
    private bool _initialized = false;

    private GameObject _sourceParent;
    private Dictionary<SoundType, Stack<AudioSource>> _idleSources; // 空闲的音频源对象
    private Dictionary<SoundType, Stack<AudioSource>> _busySources; // 正在播放的音频源对象

    public event Action<AudioSource> OnSoundPlay; // 音效播放事件
    public event Action<AudioSource> OnSoundStop; // 音效停止事件

    /// <summary>
    /// 初始化音频源对象池
    /// </summary>
    /// <param name="sourceParent">
    /// 音频源对象池GameObject的父物体，
    /// 决定音频源对象池GameObject在场景中的位置,
    /// 默认设置为SoundManager组件对应的GameObject
    /// </param>
    /// <returns> 返回SoundPool实例 </returns>
    public SoundPool Initialize([NotNull] GameObject sourceParent)
    {
        if (_initialized) return Instance;

        _initialized = true;
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

    /// <summary>
    /// 获取一个空闲的音频源对象
    /// </summary>
    /// <returns> 返回一个空闲的音频源对象 </returns>
    public AudioSource GetSource(SoundType soundType)
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
    public void RecycleBusySources(SoundType soundType)
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
    /// 扩充音频源对象池
    /// </summary>
    /// <param name="soundType"> 音频类型 </param>
    /// <param name="count"> 扩充数量，默认为10 </param>
    public void ExpandSourceList(SoundType soundType, int count = 10)
    {
        for (var i = 0; i < count; i++)
        {
            var newSourceObject = new GameObject($"AudioSource{_idleSources[soundType].Count}", new[] { typeof(AudioSource) });
            newSourceObject.transform.SetParent(_sourceParent.transform);
            _idleSources[soundType].Push(newSourceObject.GetComponent<AudioSource>());
        }
    }

    /// <summary>
    /// 扩充所有类型的音频源对象池
    /// </summary>
    /// <param name="count"> 扩充数量，默认为10 </param>
    public void ExpandSourceList(int count = 10)
    {
        foreach (var soundType in Enum.GetValues(typeof(SoundType)).Cast<SoundType>())
        {
            ExpandSourceList(soundType, count);
        }
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="sound"> 音效 </param>
    /// <param name="loop"> 是否循环播放 </param>
    public void PlaySound(SoundItem sound, bool loop = false)
    {
        var source = GetSource(sound.SoundType);
        source.clip = sound.AudioClip;
        source.loop = loop;
        source.Play();
        OnSoundPlay?.Invoke(source);
    }

    /// <summary>
    /// 停止音效
    /// </summary>
    /// <param name="sound"></param>
    public void StopSound(SoundItem sound)
    {
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
        foreach (var source in _busySources[sound.SoundType].Where(source => source.clip == sound.AudioClip))
        {
            source.UnPause();
        }
    }
}

using System;
using UnityEngine;

/// <summary>
/// 声音事件系统
/// </summary>
public class SoundEventSystem
{
    // 音频事件
    public event Action<AudioSource> OnSoundPlay;
    public event Action<AudioSource> OnSoundStop;
    public event Action<AudioSource> OnSoundPause;
    public event Action<AudioSource> OnSoundResume;

    public void TriggerSoundPlay(AudioSource source)
    {
#if VERBOSE_LOG
        Debug.Log($"[SoundEventSystem] 播放音频: {source.clip.name}");
#endif
        OnSoundPlay?.Invoke(source);
    }

    public void TriggerSoundStop(AudioSource source)
    {
#if VERBOSE_LOG
        Debug.Log($"[SoundEventSystem] 停止音频: {source.clip.name}");
#endif
        OnSoundStop?.Invoke(source);
    }

    public void TriggerSoundPause(AudioSource source)
    {
#if VERBOSE_LOG
        Debug.Log($"[SoundEventSystem] 暂停音频: {source.clip.name}");
#endif
        OnSoundPause?.Invoke(source);
    }

    public void TriggerSoundResume(AudioSource source)
    {
#if VERBOSE_LOG
        Debug.Log($"[SoundEventSystem] 恢复音频: {source.clip.name}");
#endif
        OnSoundResume?.Invoke(source);
    }
}
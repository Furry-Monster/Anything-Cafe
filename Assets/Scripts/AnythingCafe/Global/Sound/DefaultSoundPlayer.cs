using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// 默认音频播放器实现
/// </summary>
public class DefaultSoundPlayer : ISoundPlayer
{
    private readonly IAudioSourcePool _sourcePool;
    private readonly SoundEventSystem _eventSystem;

    public DefaultSoundPlayer(IAudioSourcePool sourcePool)
    {
        _sourcePool = sourcePool;
        _eventSystem = new SoundEventSystem();
    }

    public void PlaySound(SoundItem sound)
    {
        if (sound?.AudioClip == null)
        {
            Debug.LogWarning("[DefaultSoundPlayer] 尝试播放空的AudioClip");
            return;
        }

        try
        {
            var source = _sourcePool.GetSource(sound.SoundType);
            sound.OutputAudioMixerGroup = _sourcePool.GetMixerGroup(sound.SoundType);
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
            Debug.LogError($"[DefaultSoundPlayer] 播放音频失败: {e}");
        }
    }

    public void StopSound(SoundItem sound)
    {
        if (sound?.AudioClip == null) return;

        try
        {
            var sources = GetPlayingSources(sound);
            foreach (var source in sources)
            {
                source.Stop();
                _sourcePool.RecycleSource(source, sound.SoundType);
                _eventSystem.TriggerSoundStop(source);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[DefaultSoundPlayer] 停止音频失败: {e}");
        }
    }

    public void StopAllSounds()
    {
        try
        {
            _sourcePool.RecycleAllSources();
        }
        catch (Exception e)
        {
            Debug.LogError($"[DefaultSoundPlayer] 停止所有音频失败: {e}");
        }
    }

    public void PauseSound(SoundItem sound)
    {
        if (sound?.AudioClip == null) return;

        try
        {
            foreach (var source in GetPlayingSources(sound))
            {
                source.Pause();
                _eventSystem.TriggerSoundPause(source);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[DefaultSoundPlayer] 暂停音频失败: {e}");
        }
    }

    public void ResumeSound(SoundItem sound)
    {
        if (sound?.AudioClip == null) return;

        try
        {
            foreach (var source in GetPlayingSources(sound))
            {
                source.UnPause();
                _eventSystem.TriggerSoundResume(source);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[DefaultSoundPlayer] 恢复音频失败: {e}");
        }
    }

    public bool IsPlaying(SoundItem sound)
    {
        if (sound?.AudioClip == null) return false;

        try
        {
            return GetPlayingSources(sound).Any(s => s.isPlaying);
        }
        catch (Exception e)
        {
            Debug.LogError($"[DefaultSoundPlayer] 检查音频播放状态失败: {e}");
            return false;
        }
    }

    private AudioSource[] GetPlayingSources(SoundItem sound)
    {
        // 这里我们需要一个新的方法来获取正在播放指定音频的源
        // 由于我们不再直接访问_busySources，所以这个实现可能需要改变
        // 这是一个简化的实现，实际使用时可能需要更复杂的逻辑
        return GameObject.FindObjectsOfType<AudioSource>()
            .Where(s => s.clip == sound.AudioClip)
            .ToArray();
    }
}
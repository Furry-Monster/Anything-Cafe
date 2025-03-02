using System;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 音频项
/// </summary>
public class SoundItem
{
    public SoundType SoundType { get; set; }
    public AudioClip AudioClip { get; set; }
    public bool Loop { get; set; }
    public float Volume { get; set; }
    public float Pitch { get; set; }
    public float SpatialBlend { get; set; }
    public ulong Delay { get; set; }
    public AudioMixerGroup OutputAudioMixerGroup { get; set; }
    public Vector3? Position { get; set; }

    /// <summary>
    /// 创建音频项
    /// </summary>
    /// <param name="soundType">音频类型</param>
    /// <param name="audioClip">音频片段</param>
    /// <param name="loop">是否循环播放</param>
    /// <param name="volume">音量</param>
    /// <param name="pitch">音调</param>
    /// <param name="spatialBlend">空间混合</param>
    /// <param name="delay">延迟播放时间(毫秒)</param>
    /// <param name="outputAudioMixerGroup">输出混音器组</param>
    /// <param name="position">播放位置</param>
    public SoundItem(
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
        SoundType = soundType;
        AudioClip = audioClip;
        Loop = loop;
        Volume = Mathf.Clamp01(volume);
        Pitch = Mathf.Clamp(pitch, -3f, 3f);
        SpatialBlend = Mathf.Clamp01(spatialBlend);
        Delay = delay;
        OutputAudioMixerGroup = outputAudioMixerGroup;
        Position = position;
    }

    /// <summary>
    /// 将音频项的设置应用到音频源
    /// </summary>
    /// <param name="source">目标音频源</param>
    public void ApplyToSource(AudioSource source)
    {
        if (source == null)
        {
            Debug.LogError("[SoundItem] 尝试应用设置到空的AudioSource");
            return;
        }

        try
        {
            source.clip = AudioClip;
            source.loop = Loop;
            source.volume = Volume;
            source.pitch = Pitch;
            source.spatialBlend = SpatialBlend;
            source.outputAudioMixerGroup = OutputAudioMixerGroup;

            if (Position.HasValue)
            {
                source.transform.position = Position.Value;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SoundItem] 应用设置到AudioSource失败: {e}");
        }
    }
}

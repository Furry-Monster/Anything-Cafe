using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 音频源池接口
/// </summary>
public interface IAudioSourcePool
{
    /// <summary>
    /// 获取一个可用的闲置音频源
    /// </summary>
    AudioSource GetSource(SoundType soundType);

    /// <summary>
    /// 获取所有对应类型音频源
    /// </summary>
    AudioSource[] GetAllSources(SoundType soundType);

    /// <summary>
    /// 回收音频源
    /// </summary>
    void RecycleSource(AudioSource source, SoundType soundType);

    /// <summary>
    /// 获取音频源的混音器组
    /// </summary>
    AudioMixerGroup GetMixerGroup(SoundType soundType);

    /// <summary>
    /// 回收指定类型的所有音频源
    /// </summary>
    void RecycleAllSources(SoundType soundType);

    /// <summary>
    /// 回收所有音频源
    /// </summary>
    void RecycleAllSources();
}
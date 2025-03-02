using UnityEngine;

/// <summary>
/// 音频播放器接口
/// </summary>
public interface ISoundPlayer
{
    /// <summary>
    /// 播放音频
    /// </summary>
    /// <param name="sound">要播放的音频项</param>
    void PlaySound(SoundItem sound);

    /// <summary>
    /// 停止播放音频
    /// </summary>
    /// <param name="sound">要停止的音频项</param>
    void StopSound(SoundItem sound);

    /// <summary>
    /// 停止所有音频
    /// </summary>
    void StopAllSounds();

    /// <summary>
    /// 暂停音频
    /// </summary>
    /// <param name="sound">要暂停的音频项</param>
    void PauseSound(SoundItem sound);

    /// <summary>
    /// 恢复播放音频
    /// </summary>
    /// <param name="sound">要恢复的音频项</param>
    void ResumeSound(SoundItem sound);

    /// <summary>
    /// 检查音频是否正在播放
    /// </summary>
    /// <param name="sound">要检查的音频项</param>
    /// <returns>如果音频正在播放，则返回 true；否则返回 false</returns>
    bool IsPlaying(SoundItem sound);
}

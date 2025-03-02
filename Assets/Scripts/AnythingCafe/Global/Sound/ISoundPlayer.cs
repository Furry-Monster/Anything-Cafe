using UnityEngine;

/// <summary>
/// 音频播放器接口
/// </summary>
public interface ISoundPlayer
{
    void PlaySound(SoundItem sound);
    void StopSound(SoundItem sound);
    void StopAllSounds();
    void PauseSound(SoundItem sound);
    void ResumeSound(SoundItem sound);
    bool IsPlaying(SoundItem sound);
}
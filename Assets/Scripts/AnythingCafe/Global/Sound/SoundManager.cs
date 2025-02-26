using System;
using UnityEngine;

/// <summary>
/// ����������
/// </summary>
[AddComponentMenu("FrameMonster/Sound/SoundManager")]
public class SoundManager : PersistentSingleton<SoundManager>, IInitializable
{
    private SoundPool _soundPool;

    private float _globalVolumeFactor = 1; // ȫ������
    private float _musicVolumeFactor = 1; // ������������
    private float _sfxVolumeFactor = 1; // ��Ч����
    private float _ambientVolumeFactor = 1; // ��������
    private float _uiVolumeFactor = 1; // UI����
    private float _eroVolumeFactor = 1; // ɫ������

    [SerializeField]
    private GameObject _sourceParent; // ����Source��Ӧ��GameObject�ĸ�����

    public bool IsInitialized { get; set; }

    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        LoadOptions();

        // ��ʼ������SoundPool
        if (_sourceParent == null) _sourceParent = gameObject;
        _soundPool ??= SoundPool.Instance.Created(_sourceParent);
    }

    private void LoadOptions()
    {
        _globalVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.GlobalVolume);
        _musicVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.MusicVolume);
        _sfxVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.SFXVolume);
        _ambientVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.AmbientVolume);
        _uiVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.UIVolume);
        _eroVolumeFactor = OptionManager.Instance.GetValue<float>(OptionKey.EroVolume);
    }

    #region ��������,�������š�ֹͣ����ͣ���ָ�

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="type"> �������� </param>
    /// <param name="clip"> ��ƵClip </param>
    /// <param name="loop"> �Ƿ�ѭ�� </param>
    /// <param name="volume"> ���� </param>
    /// <param name="delay"> �ӳٲ��� </param>
    public void PlaySound(SoundType type, AudioClip clip, bool loop = false, float volume = 1.0f, ulong delay = 0ul)
    {
        // ��������
        var volumeFactor = type switch
        {
            SoundType.Ambient => _ambientVolumeFactor,
            SoundType.Ero => _eroVolumeFactor,
            SoundType.Music => _musicVolumeFactor,
            SoundType.SFX => _sfxVolumeFactor,
            SoundType.UI => _uiVolumeFactor,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        } * _globalVolumeFactor;

        // ���Һ��װһ��
        var soundItem = new SoundItem(type, clip, loop, volume * volumeFactor, delay);
        Debug.Log($"[SoundManager] Play sound [{type}]{clip.name} with loop {loop} and volume {volume} with delay {delay}ms");
        _soundPool.PlaySound(soundItem);
    }

    /// <summary>
    /// ֹͣ����
    /// </summary>
    /// <param name="type"> �������� </param>
    /// <param name="clip"> ��ƵClip </param>
    public void StopSound(SoundType type, AudioClip clip)
    {
        // ���Һ��װһ��
        var soundItem = new SoundItem(type, clip, false, 0, 0);
        _soundPool.StopSound(soundItem);
    }

    /// <summary>
    /// ֹͣ��������
    /// </summary>
    public void StopAllSounds() => _soundPool.StopAllSounds();

    /// <summary>
    /// ��ͣ����
    /// </summary>
    /// <param name="type"> �������� </param>
    /// <param name="clip"> ��ƵClip </param>
    public void PauseSound(SoundType type, AudioClip clip)
    {
        // ���Һ��װһ��
        var soundItem = new SoundItem(type, clip, false, 0, 0);
        _soundPool.PauseSound(soundItem);
    }

    /// <summary>
    /// �ָ�����
    /// </summary>
    /// <param name="type"> �������� </param>
    /// <param name="clip"> ��ƵClip </param>
    public void ResumeSound(SoundType type, AudioClip clip)
    {
        // ���Һ��װһ��
        var soundItem = new SoundItem(type, clip, false, 0, 0);
        _soundPool.ResumeSound(soundItem);
    }
    #endregion
}

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
///  ��ƵԴ�����
/// </summary>
public class SoundPool : Singleton<SoundPool>
{
    private bool _created = false;

    private GameObject _sourceParent;
    private Dictionary<SoundType, Stack<AudioSource>> _idleSources; // ���е���ƵԴ����
    private Dictionary<SoundType, Stack<AudioSource>> _busySources; // ���ڲ��ŵ���ƵԴ����

    public event Action<AudioSource> OnSoundPlay;// ��Ч�����¼�
    public event Action<AudioSource> OnSoundStop; // ��Чֹͣ�¼�

    /// <summary>
    /// ������ƵԴ�����
    /// </summary>
    /// <param name="sourceParent">
    /// ��ƵԴ�����GameObject�ĸ����壬
    /// ������ƵԴ�����GameObject�ڳ����е�λ��,
    /// Ĭ������ΪSoundManager�����Ӧ��GameObject
    /// </param>
    /// <returns> ����SoundPoolʵ�� </returns>
    public SoundPool Created([NotNull] GameObject sourceParent)
    {
#if VERBOSE_LOG
        Debug.Log($"[SoundPool] Initializing sound pool with source parent: {sourceParent.name}");
#endif
        if (_created) return Instance;

        _created = true;
        _sourceParent = sourceParent;

        // ��ʼ�����к����ڲ��ŵ���ƵԴ�����
        _idleSources = Enum.GetValues(typeof(SoundType))
            .Cast<SoundType>()
            .ToDictionary(type => type, _ => new Stack<AudioSource>());
        _busySources = Enum.GetValues(typeof(SoundType))
            .Cast<SoundType>()
            .ToDictionary(type => type, _ => new Stack<AudioSource>());

        ExpandSourceList();

        return Instance;
    }

    #region ˽�з���, һ�����ڹ��������߼�
    /// <summary>
    /// ��ȡһ�����е���ƵԴ����
    /// </summary>
    /// <returns> ����һ�����е���ƵԴ���� </returns>
    private AudioSource GetSource(SoundType soundType)
    {
        // ���տ��е���ƵԴ����
        RecycleBusySources(soundType);

        // ������պ���е���ƵԴ�������ȻΪ�գ���������ƵԴ�����
        if (_idleSources[soundType].Count == 0)
        {
            ExpandSourceList(soundType);
        }

        var source = _idleSources[soundType].Pop();
        _busySources[soundType].Push(source);
        return source;
    }

    /// <summary>
    /// ���տ��е�BusySource
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
    /// ����ĳһ���͵���ƵԴ�����
    /// </summary>
    /// <param name="soundType"> ��Ƶ���� </param>
    /// <param name="count"> ����������Ĭ��Ϊ10 </param>
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
    /// �����������͵���ƵԴ�����
    /// </summary>
    /// <param name="count"> ����������Ĭ��Ϊ10 </param>
    private void ExpandSourceList(int count = 10)
    {
        foreach (var soundType in Enum.GetValues(typeof(SoundType)).Cast<SoundType>())
        {
            ExpandSourceList(soundType, count);
        }
    }
    #endregion

    #region ��������, �����ⲿ����
    /// <summary>
    /// ������Ч
    /// </summary>
    /// <param name="sound"> ��Ч </param>
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
    /// ֹͣ��Ч
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
    /// ��ͣ��Ч
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
    /// �ָ���Ч
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
    /// ��������
    /// </summary>
    /// <param name="volume"> ���� </param>
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
    /// ������Ч��������
    /// </summary>
    /// <param name="soundType"> ��Ч���� </param>
    /// <param name="volume"> ���� </param>
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
    /// ������Ч�����ٶ�
    /// </summary>
    /// <param name="pitch"> �����ٶ� </param>
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
    /// ������Ч���Ͳ����ٶ�
    /// </summary>
    /// <param name="soundType"> ��Ч���� </param>
    /// <param name="pitch"> �����ٶ� </param>
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

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
    private Dictionary<SoundType, Stack<AudioSource>> _idleSources; // ���е���ƵԴ����
    private Dictionary<SoundType, Stack<AudioSource>> _busySources; // ���ڲ��ŵ���ƵԴ����

    public event Action<AudioSource> OnSoundPlay; // ��Ч�����¼�
    public event Action<AudioSource> OnSoundStop; // ��Чֹͣ�¼�

    /// <summary>
    /// ��ʼ����ƵԴ�����
    /// </summary>
    /// <param name="sourceParent">
    /// ��ƵԴ�����GameObject�ĸ����壬
    /// ������ƵԴ�����GameObject�ڳ����е�λ��,
    /// Ĭ������ΪSoundManager�����Ӧ��GameObject
    /// </param>
    /// <returns> ����SoundPoolʵ�� </returns>
    public SoundPool Initialize([NotNull] GameObject sourceParent)
    {
        if (_initialized) return Instance;

        _initialized = true;
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

    /// <summary>
    /// ��ȡһ�����е���ƵԴ����
    /// </summary>
    /// <returns> ����һ�����е���ƵԴ���� </returns>
    public AudioSource GetSource(SoundType soundType)
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
    /// ������ƵԴ�����
    /// </summary>
    /// <param name="soundType"> ��Ƶ���� </param>
    /// <param name="count"> ����������Ĭ��Ϊ10 </param>
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
    /// �����������͵���ƵԴ�����
    /// </summary>
    /// <param name="count"> ����������Ĭ��Ϊ10 </param>
    public void ExpandSourceList(int count = 10)
    {
        foreach (var soundType in Enum.GetValues(typeof(SoundType)).Cast<SoundType>())
        {
            ExpandSourceList(soundType, count);
        }
    }

    /// <summary>
    /// ������Ч
    /// </summary>
    /// <param name="sound"> ��Ч </param>
    /// <param name="loop"> �Ƿ�ѭ������ </param>
    public void PlaySound(SoundItem sound, bool loop = false)
    {
        var source = GetSource(sound.SoundType);
        source.clip = sound.AudioClip;
        source.loop = loop;
        source.Play();
        OnSoundPlay?.Invoke(source);
    }

    /// <summary>
    /// ֹͣ��Ч
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
    /// ��ͣ��Ч
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
    /// �ָ���Ч
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

using UnityEngine;

/// <summary>
/// ����������
/// </summary>
[AddComponentMenu("FrameMonster/Sound/SoundManager")]
public class SoundManager : PersistentSingleton<SoundManager>, IInitializable
{
    private SoundPool _soundPool;

    [SerializeField]
    private GameObject _sourceParent; // ����Source��Ӧ��GameObject�ĸ�����

    public bool IsInitialized { get; set; }

    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        // ��ʼ������SoundPool
        if (_sourceParent == null) _sourceParent = this.gameObject;
        _soundPool ??= SoundPool.Instance.Created(_sourceParent);
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
        // ���Һ��װһ��
        var soundItem = new SoundItem(type, clip, loop, volume, delay);
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

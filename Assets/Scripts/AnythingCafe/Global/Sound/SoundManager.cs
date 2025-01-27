using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class SoundManager : MonoBehaviourSingleton<SoundManager>
{
    private SoundPool _soundPool;
    [SerializeField] private List<SoundItem> _soundItems;

    public GameObject SourceParent;

    protected override void Awake()
    {
        base.Awake();

        if (SourceParent == null) SourceParent = this.gameObject;

        if (_soundPool == null)
        {
            _soundPool = SoundPool.Instance.Initialize(SourceParent);
        }
    }
}

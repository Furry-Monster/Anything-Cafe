using UnityEngine;

public class GlobalCanvas :
    MonoBehaviourSingleton<GlobalCanvas>,
    IInitializable
{
    [Header("Global Components")]
    [SerializeField] private SerializableDictionary<string, GlobalComponent> _globalComponents; // 全局可用的控件,在每个场景都会加载

    public bool IsInitialized { get; set; }

    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

    }

    
}

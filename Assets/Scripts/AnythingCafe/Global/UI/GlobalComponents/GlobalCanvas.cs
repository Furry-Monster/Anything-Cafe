using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalCanvas :
    MonoBehaviourSingleton<GlobalCanvas>,
    IInitializable
{
    [Header("Global Components")]
    [SerializeField] private SerializableDictionary<string, GlobalComponent> _globalComponents; // ȫ�ֿ��õĿؼ�,��ÿ�������������

    public bool IsInitialized { get; set; }

    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        CheckGlobalComponents();

        foreach (var component in _globalComponents)
            UIManager.Instance.RegisterReactiveComponent(1, component.Value, component.Key);
    }

    /// <summary>
    /// ���ȫ�����,��֤�����е�ȫ���������_globalComponents��
    /// </summary>
    private void CheckGlobalComponents()
    {
        var globalComponents = FindObjectsOfType<GlobalComponent>();
        // ���ȱʧ��ȫ�����
        foreach (var component in globalComponents)
            _globalComponents.TryAdd(component.gameObject.name, component);

        foreach (var component in _globalComponents)
            if (!globalComponents.Contains(component.Value))
                throw new CustomErrorException($"[GlobalCanvas] Global component {component.Key} is missing in scene!",
                    new CustomErrorItem(ErrorSeverity.Error, ErrorCode.ComponentNotFound));
    }
}

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
            UIManager.Instance.RegisterComponent(1, component.Value, component.Key);
    }

    /// <summary>
    /// ���ȫ�����,��֤�����е�ȫ���������_globalComponents��
    /// </summary>
    private void CheckGlobalComponents()
    {
        var sceneGlobalComponents = FindObjectsOfType<GlobalComponent>();

        // ���ȱʧ��ȫ�����
        foreach (var sceneComponent in sceneGlobalComponents)
        {
            Debug.LogWarning($"[GlobalCanvas] Found global component {sceneComponent.gameObject.name},but not in _globalComponents, adding it.");
            _globalComponents.TryAdd(sceneComponent.gameObject.name, sceneComponent);
        }

        foreach (var component in _globalComponents)
            if (!sceneGlobalComponents.Contains(component.Value))
                throw new CustomErrorException($"[GlobalCanvas] Global component {component.Key} is missing in scene!",
                    new CustomErrorItem(ErrorSeverity.Error, ErrorCode.ComponentNotFound));
    }
}

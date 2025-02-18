using System.Collections.Generic;
using System.Linq;
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

        CheckGlobalComponents();

        foreach (var component in _globalComponents)
            UIManager.Instance.RegisterComponent(1, component.Value, component.Key);
    }

    /// <summary>
    /// 检查全局组件,保证场景中的全局组件都在_globalComponents中
    /// </summary>
    private void CheckGlobalComponents()
    {
        var sceneGlobalComponents = FindObjectsOfType<GlobalComponent>();

        // 检查缺失的全局组件
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

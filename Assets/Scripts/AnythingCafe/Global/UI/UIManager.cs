using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("FrameMonster/UI/UIManager")]
public class UIManager : PersistentSingleton<UIManager>, IInitializable
{
    // string: UI名称  int: UI层级  ReactiveComponent: 控件
    [SerializeField] private SerializableDictionary<string, GameObject> _globalComponents; // 全局可用的控件,在每个场景都会加载

    [SerializeField] private SerializableDictionary<string, GameObject> _loadingComponents; // 用于加载的控件

    [SerializeField] private GraphicRaycaster _graphicRaycaster; // 用于处理UI事件的Raycaster

    private readonly Dictionary<int, List<ReactiveComponent>> _allReactiveComponents = new(); // 场景中所有的ClosableUI,int为UI层级
    private readonly LinkedList<ReactiveComponent> _activeComponents = new();  // 正在显示的UI,类似栈的机制
    private readonly List<ReactiveComponent> _closingComponents = new(); // 正在关闭的UI队列,类似GC的机制

    public ReactiveComponent CurrentActiveComponent => _activeComponents.FirstOrDefault();
    public bool IsActive(ReactiveComponent component) => _activeComponents.Contains(component);

    /// <summary>
    /// 初始化UIManager
    /// </summary>
    public void Init()
    {
        try
        {
            ValidateGlobalComponents();
            ResetAllComponents();
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException)
                throw;
            throw new CustomErrorException($"[UIManager] Can't init UIManager, {ex.Message}",
                new CustomErrorItem(ErrorSeverity.Error, ErrorCode.UIInitFailed));
        }
    }

    /// <summary>
    /// 设置GraphicRaycaster的enabled
    /// </summary>
    /// <param name="isEnabled"></param>
    public void EnableGraphicRaycaster(bool isEnabled) => _graphicRaycaster.enabled = isEnabled;

    /// <summary>
    /// 重置场景中的控件注册
    /// </summary>
    public void ResetAllComponents()
    {
        // 清除所有控件
        _allReactiveComponents.Clear();
        _activeComponents.Clear();
        _closingComponents.Clear();

        // 重新注册所有Global控件
        foreach (var component in _globalComponents.Values)
            RegisterReactiveComponent(1, component.GetComponent<ReactiveComponent>());

        // 重新注册所有Loading控件
        foreach (var component in _loadingComponents.Values)
            RegisterReactiveComponent(2, component.GetComponent<ReactiveComponent>());
    }

    /// <summary>
    /// 注册控件
    /// </summary>
    /// <param name="uiLayer"> UI层级 </param>
    /// <param name="component"> 控件 </param>
    public void RegisterReactiveComponent(int uiLayer, ReactiveComponent component)
    {
        // 整理字典
        if (!_allReactiveComponents.ContainsKey(uiLayer))
            _allReactiveComponents[uiLayer] = new List<ReactiveComponent>();
        _allReactiveComponents[uiLayer].Remove(component);
        // 初始化并注册
        if (component is IInitializable initializable)
            initializable.Init();
        _allReactiveComponents[uiLayer].Add(component);
    }

    /// <summary>
    /// 打开一个控件
    /// </summary>
    /// <param name="component"> 控件 </param>
    /// <returns> UniTask</returns>
    public async UniTask OpenReactiveComponent(ReactiveComponent component)
    {
        // 先清理掉正在关闭的控件和重复显示的控件
        _closingComponents.Remove(component);
        if (_activeComponents.Contains(component))
            return;
        // 等待显示
        _activeComponents.AddFirst(component);
        await component.Open();
    }

    /// <summary>
    /// 关闭一个控件
    /// </summary>
    /// <param name="component"> 控件 </param>
    /// <returns>UniTask</returns>
    public async UniTask CloseReactiveComponent(ReactiveComponent component)
    {
        if (!_activeComponents.Contains(component) || _closingComponents.Contains(component))
            return;
        _closingComponents.Add(component);
        _activeComponents.Remove(component);
        await component.Close();
        _closingComponents.Remove(component);
    }

    public async UniTask OpenGlobalComponent(string name)
    {

    }

    public async UniTask CloseGlobalComponent(string name)
    {

    }

    public async UniTask ShowLoadingComponent()
    {

    }

    public async UniTask HideLoadingComponent()
    {

    }

    private void ValidateGlobalComponents()
    {
        // var uiNames = _globalComponents.Keys.ToList();
        // var allGlobalComponents = GameObject.FindGameObjectsWithTag("GlobalComponent");
        // foreach (var component in allGlobalComponents)
        // {
        //     if (!uiNames.Contains(component.name) || !component.TryGetComponent(out ReactiveComponent _))
        //     {
        //         throw new CustomErrorException(
        //             $"[UIManager] Please check the global components:{component.name} is script and tag correct ?",
        //             new CustomErrorItem(ErrorSeverity.Error, ErrorCode.UIValidateFailed));
        //     }
        // }
    }
}


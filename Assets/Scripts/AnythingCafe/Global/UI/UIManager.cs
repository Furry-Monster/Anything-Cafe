using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

[AddComponentMenu("FrameMonster/UI/UIManager")]
public class UIManager : PersistentSingleton<UIManager>, IInitializable
{
    // 把Loading放在最上层，防止其他UI遮挡
    [SerializeField] private SerializableDictionary<string, LoadingComponent> _loadingComponents; // 所有可用的Loading控件

    [SerializeField] private GraphicRaycaster _graphicRaycaster;    // 用于处理UI事件的Raycaster

    private readonly Dictionary<int, List<IReactiveComponent>> _allReactiveComponents; // 场景中所有的ClosableUI,int为UI层级
    private readonly Stack<IReactiveComponent> _activeComponents;  // 正在显示的UI栈
    private readonly List<IReactiveComponent> _closingComponents; // 正在关闭的UI队列,类似GC的机制

    private LoadingComponent _currentLoadingComponent;

    public IReactiveComponent CurrentReactiveComponent => _activeComponents.Count > 0 ? _activeComponents.Peek() : null;
    public bool IsActive(IReactiveComponent component) => _activeComponents.Contains(component);
    public bool IsLoading() => _currentLoadingComponent != null;

    /// <summary>
    /// 初始化UIManager
    /// </summary>
    /// <exception cref="CustomErrorException"> 如果初始化失败 </exception>
    public void Init()
    {
        try
        {
            // TODO: 检查所有持久化控件是否被正确地添加到_allReactiveComponents
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

        // 重新注册场景中的所有控件
    }

    /// <summary>
    /// 注册控件
    /// </summary>
    /// <param name="uiLayer"> UI层级 </param>
    /// <param name="component"> 控件 </param>
    public void RegisterReactiveComponent(int uiLayer, IReactiveComponent component)
    {
        // 整理字典
        if (!_allReactiveComponents.ContainsKey(uiLayer))
            _allReactiveComponents[uiLayer] = new List<IReactiveComponent>();
        if (_allReactiveComponents[uiLayer].Contains(component))
            _allReactiveComponents[uiLayer].Remove(component);
        // 初始化并注册
        if (component is IInitializable initializable)
            initializable.Init();
        _allReactiveComponents[uiLayer].Add(component);
    }

    public async UniTask OpenReactiveComponent(IReactiveComponent component)
    {

    }

    public async UniTask CloseReactiveComponent(IReactiveComponent component)
    {

    }
}


using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("FrameMonster/UI/UIManager")]
public class UIManager : PersistentSingleton<UIManager>, IInitializable
{
    [Header("Global Components")]
    [SerializeField] private SerializableDictionary<string, GlobalComponent> _globalComponents; // 全局可用的控件,在每个场景都会加载
    [SerializeField] private GlobalCanvas _globalCanvas; // 全局Canvas
    [Space]

    [Header("Other Components")]
    [SerializeField] private GraphicRaycaster _graphicRaycaster; // 用于处理UI事件的Raycaster

    private readonly Dictionary<int, List<ReactiveComponent>> _allReactiveComponents = new(); // 场景中所有的ClosableUI,int为UI层级
    private readonly LinkedList<ReactiveComponent> _activeComponents = new();  // 正在显示的UI,类似栈的机制
    private readonly List<ReactiveComponent> _closingComponents = new(); // 正在关闭的UI队列,类似GC的机制

    public bool IsInitialized { get; set; }

    #region 常规API（初始化，重置，注册，打开，关闭）
    /// <summary>
    /// 初始化UIManager
    /// </summary>
    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        ResetMgr();
    }

    /// <summary>
    /// 重置UIManager
    /// </summary>
    /// <exception cref="CustomErrorException"> 如果重置失败则抛出异常 </exception>
    public void ResetMgr()
    {
        try
        {
            ResetCanvas();
            ResetAllComponents();
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException)
                throw;
            throw new CustomErrorException($"[UIManager] Can't reset UIManager, {ex.Message}",
                new CustomErrorItem(ErrorSeverity.Error, ErrorCode.UIResetFailed));
        }
    }


    /// <summary>
    /// 注册控件
    /// </summary>
    /// <param name="uiLayer"> UI层级 </param>
    /// <param name="component"> 控件 </param>
    public void RegisterReactiveComponent(int uiLayer, ReactiveComponent component)
    {
#if UNITY_EDITOR
        Debug.Log($"[UIManager] Register ReactiveComponent {component.name} at layer {uiLayer}");
#endif
        // 整理字典
        if (!_allReactiveComponents.ContainsKey(uiLayer))
            _allReactiveComponents[uiLayer] = new List<ReactiveComponent>();
        if (_allReactiveComponents[uiLayer].Contains(component))
            return;
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
    public async UniTask OpenReactive(ReactiveComponent component)
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
    public async UniTask CloseReactive(ReactiveComponent component)
    {
        if (!_activeComponents.Contains(component) || _closingComponents.Contains(component))
            return;
        _closingComponents.Add(component);
        _activeComponents.Remove(component);
        await component.Close();
        _closingComponents.Remove(component);
    }
    #endregion

    #region string查找方式（打开，关闭）
    /// <summary>
    /// 打开一个全局控件
    /// </summary>
    /// <param name="widgetName"> 控件名称 </param>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> 如果找不到控件则抛出异常 </exception>
    public async UniTask OpenGlobal(string widgetName)
    {
        var component = _globalComponents[widgetName];
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find global ui {widgetName}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));
        await OpenReactive(component.GetComponent<ReactiveComponent>());
    }

    public async UniTask CloseGlobal(string widgetName)
    {
        var component = _globalComponents[widgetName];
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find global ui {widgetName}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));
        await CloseReactive(component.GetComponent<ReactiveComponent>());
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 重置Canvas
    /// </summary>
    private void ResetCanvas()
    {
        if (_globalCanvas == null)
            throw new CustomErrorException("[UIManager] Canvas is set NULL!",
                new CustomErrorItem(ErrorSeverity.Error, ErrorCode.UICanvasResetFailed));
        try
        {
            // 初始化特殊Canvas
            _globalCanvas.Init();
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException)
                throw;
            throw new CustomErrorException($"[UIManager] Can't reset Canvas, {ex.Message}",
                new CustomErrorItem(ErrorSeverity.Error, ErrorCode.UICanvasResetFailed));
        }
    }

    /// <summary>
    /// 重置场景中的控件注册
    /// </summary>
    private void ResetAllComponents()
    {
        // 清除所有控件
        _allReactiveComponents.Clear();
        _activeComponents.Clear();
        _closingComponents.Clear();

        // 重新注册所有Global控件
        foreach (var component in _globalComponents.Values)
            RegisterReactiveComponent(1, component.GetComponent<ReactiveComponent>());

        
        // 寻找并注册其他控件
        var sceneComponents = FindObjectsOfType<ReactiveComponent>();
        foreach (var component in sceneComponents)
            RegisterReactiveComponent(0, component);
    }
    #endregion

    #region 泛型方式（打开，关闭）
    /// <summary>
    /// 打开一个控件
    /// </summary>
    /// <typeparam name="T"> 控件类型 </typeparam>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> 如果找不到控件则抛出异常 </exception>
    public async UniTask OpenReactive<T>() where T : ReactiveComponent
    {
        var component = _allReactiveComponents.Values
            .SelectMany(c => c)
            .FirstOrDefault(c => c.GetComponent<T>() != null);
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find ui {typeof(T).Name}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));
        await OpenReactive(component);
    }

    /// <summary>
    /// 打开一个全局控件，同时不注入数据模板
    /// </summary>
    /// <typeparam name="T"> 控件类型 </typeparam>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> 如果找不到控件则抛出异常 </exception>
    public async UniTask OpenGlobal<T>() where T : ReactiveComponent
    {
        var component = _globalComponents.Values
            .FirstOrDefault(c => c.GetComponent<T>() != null);
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find global ui {typeof(T).Name}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));
        await OpenReactive(component.GetComponent<ReactiveComponent>());
    }
    #endregion

    #region 泛型方式（With DataTemplate）
    /// <summary>
    /// 注入数据模板
    /// </summary>
    /// <typeparam name="T"> 控件类型 </typeparam>
    /// <typeparam name="TData"> 数据模板类型 </typeparam>
    /// <param name="data"> 数据模板 </param>
    /// <returns> ReactiveComponent </returns>
    /// <exception cref="CustomErrorException"> 如果找不到控件则抛出异常 </exception>
    public ReactiveComponent LoadDataReactive<T, TData>(TData data)
        where T : ReactiveComponent
        where TData : IDataTemplate
    {
        var component = _allReactiveComponents.Values
            .SelectMany(c => c)
            .FirstOrDefault(c => c.GetComponent<T>() != null);
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find ui {typeof(T).Name}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));

        // 加载数据模板
        if (component is IHasDataTemplate<TData> dataComponent)
            dataComponent.LoadTemplate(data);
        else
            Debug.LogWarning($"[UIManager] {component.name} doesn't implement IHasDataTemplate<TData>!");

        return component;
    }

    /// <summary>
    /// 注入数据模板打开一个控件
    /// </summary>
    /// <typeparam name="T"> 控件类型 </typeparam>
    /// <typeparam name="TData"> 数据模板类型 </typeparam>
    /// <param name="data"> 数据模板 </param>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> 如果找不到控件则抛出异常 </exception>
    public async UniTask OpenReactive<T, TData>(TData data)
        where T : ReactiveComponent
        where TData : IDataTemplate
    {
        var component = LoadDataReactive<T, TData>(data);

        await OpenReactive(component);
    }

    /// <summary>
    /// 注入数据模板打开一个全局控件
    /// </summary>
    /// <typeparam name="T"> 控件类型 </typeparam>
    /// <typeparam name="TData"> 数据模板类型 </typeparam>
    /// <param name="data"> 数据模板 </param>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> 如果找不到控件则抛出异常 </exception>
    public async UniTask OpenGlobal<T, TData>(TData data)
        where T : ReactiveComponent
        where TData : IDataTemplate
    {
        var component = _globalComponents.Values
            .FirstOrDefault(c => c.GetComponent<T>() != null);
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find global ui {typeof(T).Name}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));

        // 加载数据模板
        if (component is IHasDataTemplate<TData> dataComponent)
            dataComponent.LoadTemplate(data);
        else
            Debug.LogWarning($"[UIManager] {component.name} doesn't implement IHasDataTemplate<TData>!");

        await OpenReactive(component.GetComponent<ReactiveComponent>());
    }
    #endregion
}


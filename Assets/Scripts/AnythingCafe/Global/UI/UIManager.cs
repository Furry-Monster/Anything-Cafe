using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("FrameMonster/UI/UIManager")]
public class UIManager : PersistentSingleton<UIManager>, IInitializable
{
    // string: UI����  int: UI�㼶  ReactiveComponent: �ؼ�
    [Header("Global Components")]
    [SerializeField] private SerializableDictionary<string, GlobalComponent> _globalComponents; // ȫ�ֿ��õĿؼ�,��ÿ�������������
    [SerializeField] private GlobalCanvas _globalCanvas; // ȫ��Canvas
    [Space]

    [Header("Loading Components")]
    [SerializeField] private SerializableDictionary<string, LoadingComponent> _loadingComponents; // ���ڼ��صĿؼ�
    [SerializeField] private LoadingCanvas _loadingCanvas;// ���ڼ���ҳ���Canvas
    [Space]

    [Header("Other Components")]
    [SerializeField] private GraphicRaycaster _graphicRaycaster; // ���ڴ���UI�¼���Raycaster

    private readonly Dictionary<int, List<ReactiveComponent>> _allReactiveComponents = new(); // ���������е�ClosableUI,intΪUI�㼶
    private readonly LinkedList<ReactiveComponent> _activeComponents = new();  // ������ʾ��UI,����ջ�Ļ���
    private readonly List<ReactiveComponent> _closingComponents = new(); // ���ڹرյ�UI����,����GC�Ļ���

    public bool IsInitialized { get; set; }

    /// <summary>
    /// ��ʼ��UIManager
    /// </summary>
    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        try
        {
            ResetCanvas();
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

    #region API�����ã�ע�ᣬ�򿪣��رգ�
    /// <summary>
    /// ����Canvas
    /// </summary>
    public void ResetCanvas()
    {
        if (_globalCanvas == null || _loadingCanvas == null)
            throw new CustomErrorException("[UIManager] Canvas is set NULL!",
                new CustomErrorItem(ErrorSeverity.Error, ErrorCode.UICanvasResetFailed));
        try
        {
            // ��ʼ������Canvas
            _globalCanvas.Init();
            _loadingCanvas.Init();

            // ���ȱʧ�Ŀؼ�
            _globalCanvas.CheckComponents(_globalComponents.Values.ToList<ReactiveComponent>());
            _loadingCanvas.CheckComponents(_loadingComponents.Values.ToList<ReactiveComponent>());
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
    /// ���ó����еĿؼ�ע��
    /// </summary>
    public void ResetAllComponents()
    {
        // ������пؼ�
        _allReactiveComponents.Clear();
        _activeComponents.Clear();
        _closingComponents.Clear();

        // ����ע������Global�ؼ�
        foreach (var component in _globalComponents.Values)
            RegisterReactiveComponent(1, component.GetComponent<ReactiveComponent>());

        // ����ע������Loading�ؼ�
        foreach (var component in _loadingComponents.Values)
            RegisterReactiveComponent(2, component.GetComponent<ReactiveComponent>());

        // Ѱ�Ҳ�ע�������ؼ�
        var sceneComponents = FindObjectsOfType<ReactiveComponent>();
        foreach (var component in sceneComponents)
            RegisterReactiveComponent(0, component);
    }

    /// <summary>
    /// ע��ؼ�
    /// </summary>
    /// <param name="uiLayer"> UI�㼶 </param>
    /// <param name="component"> �ؼ� </param>
    public void RegisterReactiveComponent(int uiLayer, ReactiveComponent component)
    {
#if UNITY_EDITOR
        Debug.Log($"[UIManager] Register ReactiveComponent {component.name} at layer {uiLayer}");
#endif
        // �����ֵ�
        if (!_allReactiveComponents.ContainsKey(uiLayer))
            _allReactiveComponents[uiLayer] = new List<ReactiveComponent>();
        if (_allReactiveComponents[uiLayer].Contains(component))
            return;
        // ��ʼ����ע��
        if (component is IInitializable initializable)
            initializable.Init();
        _allReactiveComponents[uiLayer].Add(component);
    }

    /// <summary>
    /// ��һ���ؼ�
    /// </summary>
    /// <param name="component"> �ؼ� </param>
    /// <returns> UniTask</returns>
    public async UniTask OpenReactive(ReactiveComponent component)
    {
        // ����������ڹرյĿؼ����ظ���ʾ�Ŀؼ�
        _closingComponents.Remove(component);
        if (_activeComponents.Contains(component))
            return;
        // �ȴ���ʾ
        _activeComponents.AddFirst(component);
        await component.Open();
    }

    /// <summary>
    /// �ر�һ���ؼ�
    /// </summary>
    /// <param name="component"> �ؼ� </param>
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

    /// <summary>
    /// ��ȫ��UI
    /// </summary>
    /// <param name="uiName"> UI���� </param>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> ����Ҳ����ؼ����׳��쳣 </exception>
    public async UniTask OpenGlobal(string uiName)
    {
        var component = _globalComponents[uiName];
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find global ui {uiName}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));
        await OpenReactive(component.GetComponent<ReactiveComponent>());
    }

    /// <summary>
    ///  ��Loading UI
    /// </summary>
    /// <param name="uiName"> UI���� </param>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> ����Ҳ����ؼ����׳��쳣 </exception>
    public async UniTask OpenLoading(string uiName)
    {
        var component = _loadingComponents[uiName];
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find loading ui {uiName}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));
        await OpenReactive(component.GetComponent<ReactiveComponent>());
    }

    /// <summary>
    ///  �ر�ȫ��UI
    /// </summary>
    /// <param name="uiName"> UI���� </param>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> ����Ҳ����ؼ����׳��쳣 </exception>
    public async UniTask CloseGlobal(string uiName)
    {
        var component = _globalComponents[uiName];
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find global ui {uiName}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));
        await CloseReactive(component.GetComponent<ReactiveComponent>());
    }

    /// <summary>
    ///  �ر�Loading UI
    /// </summary>
    /// <param name="uiName"> UI���� </param>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> ����Ҳ����ؼ����׳��쳣 </exception>
    public async UniTask CloseLoading(string uiName)
    {
        var component = _loadingComponents[uiName];
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find loading ui {uiName}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));
        await CloseReactive(component.GetComponent<ReactiveComponent>());
    }
    #endregion

    #region ��ݷ�ʽ���򿪣��رգ�
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

    public async UniTask OpenGlobal<T>() where T : ReactiveComponent
    {
        var component = _globalComponents.Values
            .FirstOrDefault(c => c.GetComponent<T>() != null);
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find global ui {typeof(T).Name}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));
        await OpenReactive(component.GetComponent<ReactiveComponent>());
    }

    public async UniTask OpenLoading<T>() where T : ReactiveComponent
    {
        var component = _loadingComponents.Values
            .FirstOrDefault(c => c.GetComponent<T>() != null);
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find loading ui {typeof(T).Name}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));
        await OpenReactive(component.GetComponent<ReactiveComponent>());
    }

    public async UniTask CloseReactive<T>() where T : ReactiveComponent
    {
        var component = _allReactiveComponents.Values
            .SelectMany(c => c)
            .FirstOrDefault(c => c.GetComponent<T>() != null);
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find ui {typeof(T).Name}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));
        await CloseReactive(component);
    }

    public async UniTask CloseGlobal<T>() where T : ReactiveComponent
    {
        var component = _globalComponents.Values
            .FirstOrDefault(c => c.GetComponent<T>() != null);
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find global ui {typeof(T).Name}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));
        await CloseReactive(component.GetComponent<ReactiveComponent>());
    }

    public async UniTask CloseLoading<T>() where T : ReactiveComponent
    {
        var component = _loadingComponents.Values
            .FirstOrDefault(c => c.GetComponent<T>() != null);
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find loading ui {typeof(T).Name}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));
        await CloseReactive(component.GetComponent<ReactiveComponent>());
    }
    #endregion

    #region ��ݷ�ʽ��With DataTemplate��

    public async UniTask OpenReactive<T, TData>(TData data)
        where T : ReactiveComponent
        where TData : IDataTemplate, new()
    {
        var component = _allReactiveComponents.Values
            .SelectMany(c => c)
            .FirstOrDefault(c => c.GetComponent<T>() != null);
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find ui {typeof(T).Name}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));

        // ��������ģ��
        if (component is IHasDataTemplate<TData> dataComponent)
            dataComponent.LoadTemplate(data);
        else
            Debug.LogWarning($"[UIManager] {component.name} doesn't implement IHasDataTemplate<TData>!");

        await OpenReactive(component);
    }

    public async UniTask OpenGlobal<T, TData>(TData data)
        where T : ReactiveComponent
        where TData : IDataTemplate, new()
    {
        var component = _globalComponents.Values
            .FirstOrDefault(c => c.GetComponent<T>() != null);
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find global ui {typeof(T).Name}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));

        // ��������ģ��
        if (component is IHasDataTemplate<TData> dataComponent)
            dataComponent.LoadTemplate(data);
        else
            Debug.LogWarning($"[UIManager] {component.name} doesn't implement IHasDataTemplate<TData>!");

        await OpenReactive(component.GetComponent<ReactiveComponent>());
    }

    public async UniTask OpenLoading<T, TData>(TData data)
        where T : ReactiveComponent
        where TData : IDataTemplate, new()
    {
        var component = _loadingComponents.Values
            .FirstOrDefault(c => c.GetComponent<T>());
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find loading ui {typeof(T).Name}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));

        // ��������ģ��
        if (component is IHasDataTemplate<TData> dataComponent)
            dataComponent.LoadTemplate(data);
        else
            Debug.LogWarning($"[UIManager] {component.name} doesn't implement IHasDataTemplate<TData>!");

        await OpenReactive(component.GetComponent<ReactiveComponent>());
    }
    #endregion
}


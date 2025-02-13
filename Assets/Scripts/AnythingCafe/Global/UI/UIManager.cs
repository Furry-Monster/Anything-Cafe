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

    #region API����ʼ�������ã�ע�ᣬ�򿪣��رգ�
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

    /// <summary>
    /// ����UIManager
    /// </summary>
    /// <exception cref="CustomErrorException"> �������ʧ�����׳��쳣 </exception>
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
    // TODO:������ķ����Ż���
    #endregion

    #region ˽�з���
    /// <summary>
    /// ����Canvas
    /// </summary>
    private void ResetCanvas()
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
    private void ResetAllComponents()
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
    #endregion

    #region ���ͷ�ʽ���򿪣��رգ�
    /// <summary>
    /// ��һ���ؼ�
    /// </summary>
    /// <typeparam name="T"> �ؼ����� </typeparam>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> ����Ҳ����ؼ����׳��쳣 </exception>
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
    /// ��һ��ȫ�ֿؼ���ͬʱ��ע������ģ��
    /// </summary>
    /// <typeparam name="T"> �ؼ����� </typeparam>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> ����Ҳ����ؼ����׳��쳣 </exception>
    public async UniTask OpenGlobal<T>() where T : ReactiveComponent
    {
        var component = _globalComponents.Values
            .FirstOrDefault(c => c.GetComponent<T>() != null);
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find global ui {typeof(T).Name}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));
        await OpenReactive(component.GetComponent<ReactiveComponent>());
    }

    /// <summary>
    /// ��һ��Loading�ؼ�,ͬʱ��ע������ģ��
    /// </summary>
    /// <typeparam name="T"> �ؼ����� </typeparam>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> ����Ҳ����ؼ����׳��쳣 </exception>
    public async UniTask OpenLoading<T>() where T : ReactiveComponent
    {
        var component = _loadingComponents.Values
            .FirstOrDefault(c => c.GetComponent<T>() != null);
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find loading ui {typeof(T).Name}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));
        await OpenReactive(component.GetComponent<ReactiveComponent>());
    }

    /// <summary>
    /// �ر�һ���ؼ�
    /// </summary>
    /// <typeparam name="T"> �ؼ����� </typeparam>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> ����Ҳ����ؼ����׳��쳣 </exception>
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

    /// <summary>
    /// �ر�һ��ȫ�ֿؼ�
    /// </summary>
    /// <typeparam name="T"> �ؼ����� </typeparam>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> ����Ҳ����ؼ����׳��쳣 </exception>
    public async UniTask CloseGlobal<T>() where T : ReactiveComponent
    {
        var component = _globalComponents.Values
            .FirstOrDefault(c => c.GetComponent<T>() != null);
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find global ui {typeof(T).Name}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));
        await CloseReactive(component.GetComponent<ReactiveComponent>());
    }

    /// <summary>
    /// �ر�һ��Loading�ؼ�
    /// </summary>
    /// <typeparam name="T"> �ؼ����� </typeparam>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> ����Ҳ����ؼ����׳��쳣 </exception>
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

    #region ���ͷ�ʽ��With DataTemplate��
    /// <summary>
    /// ע������ģ���һ���ؼ�
    /// </summary>
    /// <typeparam name="T"> �ؼ����� </typeparam>
    /// <typeparam name="TData"> ����ģ������ </typeparam>
    /// <param name="data"> ����ģ�� </param>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> ����Ҳ����ؼ����׳��쳣 </exception>
    public async UniTask OpenReactive<T, TData>(TData data)
        where T : ReactiveComponent
        where TData : IDataTemplate
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

    /// <summary>
    /// ע������ģ���һ��ȫ�ֿؼ�
    /// </summary>
    /// <typeparam name="T"> �ؼ����� </typeparam>
    /// <typeparam name="TData"> ����ģ������ </typeparam>
    /// <param name="data"> ����ģ�� </param>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> ����Ҳ����ؼ����׳��쳣 </exception>
    public async UniTask OpenGlobal<T, TData>(TData data)
        where T : ReactiveComponent
        where TData : IDataTemplate
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

    /// <summary>
    /// ע������ģ���һ��Loading�ؼ�
    /// </summary>
    /// <typeparam name="T"> �ؼ����� </typeparam>
    /// <typeparam name="TData"> ����ģ������ </typeparam>
    /// <param name="data"> ����ģ�� </param>
    /// <returns> UniTask </returns>
    /// <exception cref="CustomErrorException"> ����Ҳ����ؼ����׳��쳣 </exception>
    public async UniTask OpenLoading<T, TData>(TData data)
        where T : ReactiveComponent
        where TData : IDataTemplate
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


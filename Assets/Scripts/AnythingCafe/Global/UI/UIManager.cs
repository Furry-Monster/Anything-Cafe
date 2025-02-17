using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("FrameMonster/UI/UIManager")]
public class UIManager : PersistentSingleton<UIManager>, IInitializable
{
    [Header("Global Components")]
    [SerializeField] private GlobalCanvas _globalCanvas;

    private readonly Dictionary<int, Dictionary<string, ReactiveComponent>> _allReactiveComponents = new(); // ���������е�ClosableUI,intΪUI�㼶
    private readonly LinkedList<ReactiveComponent> _activeComponents = new();  // ������ʾ��UI,����ջ�Ļ���
    private readonly List<ReactiveComponent> _closingComponents = new(); // ���ڹرյ�UI����,����GC�Ļ���

    public bool IsInitialized { get; set; }

    #region ����API����ʼ�������ã�ע�ᣬ�򿪣��رգ�
    /// <summary>
    /// ��ʼ��UIManager
    /// </summary>
    public void Init()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        ResetMgr();
    }

    /// <summary>
    /// ����UIManager
    /// </summary>
    /// <exception cref="CustomErrorException"> �������ʧ�����׳��쳣 </exception>
    public void ResetMgr()
    {
        try
        {
            ResetAllComponents();
            ReRegisterGlobal();
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
    /// <param name="widgetName"> �ؼ����� </param>
    public void RegisterComponent(int uiLayer, ReactiveComponent component, string widgetName = null)
    {
#if UNITY_EDITOR
        Debug.Log($"[UIManager] Register ReactiveComponent {component.name} at layer {uiLayer}");
#endif
        // �����ֵ�
        if (!_allReactiveComponents.ContainsKey(uiLayer))
            _allReactiveComponents[uiLayer] = new Dictionary<string, ReactiveComponent>();
        if (_allReactiveComponents[uiLayer].Values.Contains(component))
            return;
        // ��ʼ����ע��
        if (component is IInitializable initializable)
            initializable.Init();
        _allReactiveComponents[uiLayer].Add(widgetName ?? component.name, component);
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
    #endregion

    #region ˽�з���������Canvas���������пؼ�ע�ᣩ
    /// <summary>
    /// ���ó����еĿؼ�ע��
    /// </summary>
    private void ResetAllComponents()
    {
        // ������пؼ�
        _allReactiveComponents.Clear();
        _activeComponents.Clear();
        _closingComponents.Clear();
    }

    /// <summary>
    /// ����Canvas
    /// </summary>
    private void ReRegisterGlobal()
    {
        if (_globalCanvas == null)
        {
            _globalCanvas = FindObjectOfType<GlobalCanvas>();
            if (_globalCanvas == null)
                throw new CustomErrorException($"[UIManager] Can't find GlobalCanvas!",
                    new CustomErrorItem(ErrorSeverity.Error, ErrorCode.UIResetFailed));
        }

        try
        {
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
        var component = _allReactiveComponents[0].Values
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
        var globalComponents = _allReactiveComponents[1].Values
            .FirstOrDefault(c => c.GetComponent<T>() != null);
        if (globalComponents == null)
            throw new CustomErrorException($"[UIManager] Can't find global ui {typeof(T).Name}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));
        await OpenReactive(globalComponents.GetComponent<ReactiveComponent>());
    }
    #endregion

    #region ģ��ע�뷽ʽ��With DataTemplate��
    /// <summary>
    /// ע������ģ��
    /// </summary>
    /// <typeparam name="T"> �ؼ����� </typeparam>
    /// <typeparam name="TData"> ����ģ������ </typeparam>
    /// <param name="data"> ����ģ�� </param>
    /// <returns> ReactiveComponent </returns>
    /// <exception cref="CustomErrorException"> ����Ҳ����ؼ����׳��쳣 </exception>
    public ReactiveComponent LoadDataReactive<T, TData>(TData data)
        where T : ReactiveComponent
        where TData : IDataTemplate
    {
        var component = _allReactiveComponents[0].Values
            .FirstOrDefault(c => c.GetComponent<T>() != null);
        if (component == null)
            throw new CustomErrorException($"[UIManager] Can't find ui {typeof(T).Name}!",
                new CustomErrorItem(ErrorSeverity.Warning, ErrorCode.ComponentNotFound));

        // ��������ģ��
        if (component is IHasDataTemplate<TData> dataComponent)
            dataComponent.LoadTemplate(data);
        else
            Debug.LogWarning($"[UIManager] {component.name} doesn't implement IHasDataTemplate<TData>!");

        return component;
    }

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
        var component = LoadDataReactive<T, TData>(data);

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
        var component = _allReactiveComponents[1].Values
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
    #endregion
}


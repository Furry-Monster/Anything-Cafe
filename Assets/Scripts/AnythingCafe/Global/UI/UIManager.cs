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
    [SerializeField] private SerializableDictionary<string, GameObject> _globalComponents; // ȫ�ֿ��õĿؼ�,��ÿ�������������

    [SerializeField] private SerializableDictionary<string, GameObject> _loadingComponents; // ���ڼ��صĿؼ�

    [SerializeField] private GraphicRaycaster _graphicRaycaster; // ���ڴ���UI�¼���Raycaster

    private readonly Dictionary<int, List<ReactiveComponent>> _allReactiveComponents = new(); // ���������е�ClosableUI,intΪUI�㼶
    private readonly LinkedList<ReactiveComponent> _activeComponents = new();  // ������ʾ��UI,����ջ�Ļ���
    private readonly List<ReactiveComponent> _closingComponents = new(); // ���ڹرյ�UI����,����GC�Ļ���

    public ReactiveComponent CurrentActiveComponent => _activeComponents.FirstOrDefault();
    public bool IsActive(ReactiveComponent component) => _activeComponents.Contains(component);

    /// <summary>
    /// ��ʼ��UIManager
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
    /// ����GraphicRaycaster��enabled
    /// </summary>
    /// <param name="isEnabled"></param>
    public void EnableGraphicRaycaster(bool isEnabled) => _graphicRaycaster.enabled = isEnabled;

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
    }

    /// <summary>
    /// ע��ؼ�
    /// </summary>
    /// <param name="uiLayer"> UI�㼶 </param>
    /// <param name="component"> �ؼ� </param>
    public void RegisterReactiveComponent(int uiLayer, ReactiveComponent component)
    {
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
    public async UniTask OpenReactiveComponent(ReactiveComponent component)
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
        var globalUINames = _globalComponents.Keys.ToList();
        var allGlobalComponents = GameObject.FindGameObjectsWithTag("GlobalComponent");
        foreach (var component in allGlobalComponents)
        {
            if (!globalUINames.Contains(component.name) || !component.TryGetComponent(out ReactiveComponent _))
            {
                throw new CustomErrorException(
                    $"[UIManager] Please check the global components:{component.name} is script and tag correct ?",
                    new CustomErrorItem(ErrorSeverity.Error, ErrorCode.UIValidateFailed));
            }
        }

        var loadingUINames = _loadingComponents.Keys.ToList();
        var allLoadingComponents = GameObject.FindGameObjectsWithTag("LoadingComponent");
        foreach (var component in allLoadingComponents)
        {
            if (!loadingUINames.Contains(component.name) || !component.TryGetComponent(out ReactiveComponent _))
            {
                throw new CustomErrorException(
                    $"[UIManager] Please check the loading components:{component.name} is script and tag correct ?",
                    new CustomErrorItem(ErrorSeverity.Error, ErrorCode.UIValidateFailed));
            }
        }
    }
}


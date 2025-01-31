using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

[AddComponentMenu("FrameMonster/UI/UIManager")]
public class UIManager : PersistentSingleton<UIManager>, IInitializable
{
    // ��Loading�������ϲ㣬��ֹ����UI�ڵ�
    [SerializeField] private SerializableDictionary<string, LoadingComponent> _loadingComponents; // ���п��õ�Loading�ؼ�

    [SerializeField] private GraphicRaycaster _graphicRaycaster;    // ���ڴ���UI�¼���Raycaster

    private readonly Dictionary<int, List<IReactiveComponent>> _allReactiveComponents; // ���������е�ClosableUI,intΪUI�㼶
    private readonly Stack<IReactiveComponent> _activeComponents;  // ������ʾ��UIջ
    private readonly List<IReactiveComponent> _closingComponents; // ���ڹرյ�UI����,����GC�Ļ���

    private LoadingComponent _currentLoadingComponent;

    public IReactiveComponent CurrentReactiveComponent => _activeComponents.Count > 0 ? _activeComponents.Peek() : null;
    public bool IsActive(IReactiveComponent component) => _activeComponents.Contains(component);
    public bool IsLoading() => _currentLoadingComponent != null;

    /// <summary>
    /// ��ʼ��UIManager
    /// </summary>
    /// <exception cref="CustomErrorException"> �����ʼ��ʧ�� </exception>
    public void Init()
    {
        try
        {
            // TODO: ������г־û��ؼ��Ƿ���ȷ����ӵ�_allReactiveComponents
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

        // ����ע�᳡���е����пؼ�
    }

    /// <summary>
    /// ע��ؼ�
    /// </summary>
    /// <param name="uiLayer"> UI�㼶 </param>
    /// <param name="component"> �ؼ� </param>
    public void RegisterReactiveComponent(int uiLayer, IReactiveComponent component)
    {
        // �����ֵ�
        if (!_allReactiveComponents.ContainsKey(uiLayer))
            _allReactiveComponents[uiLayer] = new List<IReactiveComponent>();
        if (_allReactiveComponents[uiLayer].Contains(component))
            _allReactiveComponents[uiLayer].Remove(component);
        // ��ʼ����ע��
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


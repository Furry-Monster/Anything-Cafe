using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("FrameMonster/UI/UIManager")]
public class UIManager : PersistentSingleton<UIManager>
{
    [SerializeField] private SerializableDictionary<string, ClosableComponent> _persistentComponents; // ���п��ó־û��Ŀؼ�����Ҫ����UIManager�����£�������Inspector���ֶ����

    [SerializeField] private SerializableDictionary<string, LoadingComponent> _loadingComponents; // ���п��õ�Loading�ؼ�

    [SerializeField] private GraphicRaycaster _graphicRaycaster;    // ���ڴ���UI�¼���Raycaster

    private readonly List<ClosableComponent> _allClosableComponents; // ���������е�ClosableUI
    private readonly Stack<ClosableComponent> _activeComponents;  // ������ʾ��UIջ
    private readonly List<ClosableComponent> _closingComponents; // ���ڹرյ�UI����,����GC�Ļ���
    private LoadingComponent _currentLoadingComponent;

    public ClosableComponent CurrentClosableComponent
    {
        get
        {
            try
            {
                return _activeComponents.Peek();
            }
            catch
            {
                return null;
            }
        }
    }

    public bool IsLoading { get; private set; }

    protected override void Awake()
    {
        base.Awake();


    }

}

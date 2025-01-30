using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("FrameMonster/UI/UIManager")]
public class UIManager : PersistentSingleton<UIManager>
{
    [SerializeField] private SerializableDictionary<string, ClosableComponent> _persistentComponents; // 所有可用持久化的控件，需要放在UIManager物体下，或者在Inspector中手动添加

    [SerializeField] private SerializableDictionary<string, LoadingComponent> _loadingComponents; // 所有可用的Loading控件

    [SerializeField] private GraphicRaycaster _graphicRaycaster;    // 用于处理UI事件的Raycaster

    private readonly List<ClosableComponent> _allClosableComponents; // 场景中所有的ClosableUI
    private readonly Stack<ClosableComponent> _activeComponents;  // 正在显示的UI栈
    private readonly List<ClosableComponent> _closingComponents; // 正在关闭的UI队列,类似GC的机制
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

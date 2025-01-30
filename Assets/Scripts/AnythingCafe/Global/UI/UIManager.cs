using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("FrameMonster/UI/UIManager")]
public class UIManager : PersistentSingleton<UIManager>
{
    

    [SerializeField] private SerializableDictionary<LoadingUIType, LoadingUI> _loadingUIs; // 所有可用的LoadingUI

    [SerializeField] private GraphicRaycaster _graphicRaycaster;    // 用于处理UI事件的Raycaster

    private readonly List<ClosableUI> _allClosableUIs; // 场景中所有的ClosableUI
    private readonly Stack<ClosableUI> _activeUIs;  // 正在显示的UI栈
    private readonly List<ClosableUI> _closingUIs; // 正在关闭的UI队列,类似GC的机制



}

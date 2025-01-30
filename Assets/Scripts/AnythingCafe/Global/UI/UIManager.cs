using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("FrameMonster/UI/UIManager")]
public class UIManager : PersistentSingleton<UIManager>
{
    

    [SerializeField] private SerializableDictionary<LoadingUIType, LoadingUI> _loadingUIs; // ���п��õ�LoadingUI

    [SerializeField] private GraphicRaycaster _graphicRaycaster;    // ���ڴ���UI�¼���Raycaster

    private readonly List<ClosableUI> _allClosableUIs; // ���������е�ClosableUI
    private readonly Stack<ClosableUI> _activeUIs;  // ������ʾ��UIջ
    private readonly List<ClosableUI> _closingUIs; // ���ڹرյ�UI����,����GC�Ļ���



}

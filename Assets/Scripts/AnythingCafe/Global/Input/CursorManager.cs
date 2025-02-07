using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CursorManager :
    PersistentSingleton<CursorManager>,
    IInitializable
{
    [SerializeField]
    private SerializableDictionary<CursorID, CursorItem> _cursorDict;

    private Dictionary<CursorLayer, CursorID> _currentCursorDict;
    private int _cursorFrame;
    private float _frameTimer;

    private Action _onCursorMgrUpdate;

    public CursorItem CurrentCursor
    {
        get
        {
            var lastLayer = _currentCursorDict
                .LastOrDefault(kvp => kvp.Value != CursorID.None);

            return lastLayer.Equals(default(KeyValuePair<CursorLayer, CursorID>))
                ? _cursorDict[CursorID.Default]
                : _cursorDict[lastLayer.Value];
        }
    }

#if ENABLE_LEGACY_INPUT_MANAGER // �����������
    public Vector2 CursorPosition => Input.mousePosition;

    public Vector2 CursorWorldPosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);
#elif ENABLE_INPUT_SYSTEM // ������ϵͳ
    public Vector2 CursorPosition => Mouse.current.position.ReadValue();

    public Vector2 CursorWorldPosition => Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
#endif

    public async void Init()
    {
        try
        {
            await InitializeCursorDictionary();
            SetCursor(CursorID.Default, CursorLayer.Base);
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException)
                throw;
            throw new CustomErrorException("[CursorManager] LoadTemplate failed",
                new CustomErrorItem(ErrorSeverity.Error, ErrorCode.CursorManagerInitFailed));
        }
    }

    /// <summary>
    /// ����ָ����Ĺ��
    /// </summary>
    /// <param name="targetCursor"> ָ���Ĺ��SO </param>
    /// <param name="cursorLayer"> ���㼶 </param>
    public void SetCursor(CursorID targetCursor, CursorLayer cursorLayer = CursorLayer.Temp)
    {
        if (_currentCursorDict[cursorLayer] == targetCursor)
            return;
        _currentCursorDict[cursorLayer] = targetCursor;

        // ���������£�
        _onCursorMgrUpdate -= UpdateCursorAnimation;
        // �л����ͼ�꣬�����ö���֡��
        _cursorFrame = 0;
        _frameTimer = CurrentCursor.FrameRate;
        Cursor.SetCursor(CurrentCursor.CursorTextures[0], CurrentCursor.CursorOffset, CursorMode.ForceSoftware);
        Cursor.visible = true;
        UpdateCursorAnimation();
        // ������ж�������ע������¼�������ֻ������һ�����ͼ��
        if (CurrentCursor.FrameCount > 1)
            _onCursorMgrUpdate += UpdateCursorAnimation;
    }

    /// <summary>
    /// ����ָ����Ĺ��
    /// </summary>
    /// <param name="cursorLayer"> ���㼶 </param>
    public void ResetCursor(CursorLayer cursorLayer = CursorLayer.Temp) => SetCursor(CursorID.None, cursorLayer);

    private void Update() => _onCursorMgrUpdate?.Invoke();

    /// <summary>
    /// �첽��ʼ������ֵ�,���ع��Ԥ������Դ
    /// </summary>
    /// <returns></returns>
    private async UniTask InitializeCursorDictionary()
    {
        _currentCursorDict = Enum.GetValues(typeof(CursorLayer))
            .Cast<CursorLayer>()
            .ToDictionary(layer => layer, _ => CursorID.None);

        // TODO: ������������ӹ����Դ��Ԥ�����߼�
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// ���¹�궯��
    /// </summary>
    private void UpdateCursorAnimation()
    {
        _frameTimer -= Time.deltaTime;

        // �����궯��������ϣ��л�����һ֡
        if (_frameTimer.CompareTo(0.0f) > 0) return;
        _cursorFrame = (_cursorFrame + 1) % CurrentCursor.FrameCount;
        _frameTimer = CurrentCursor.FrameRate;
        Cursor.SetCursor(CurrentCursor.CursorTextures[_cursorFrame], CurrentCursor.CursorOffset, CursorMode.ForceSoftware);
    }
}


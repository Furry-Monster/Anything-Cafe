using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("FrameMonster/Input/CursorManager")]
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

    public CursorItem CurrentCursor => _cursorDict[
        _currentCursorDict
            .Last(keyValuePair => keyValuePair.Value != CursorID.None)
            .Value
    ];

#if ENABLE_LEGACY_INPUT_MANAGER // 如果使用了旧的输入管理器
    public Vector2 CursorPosition => Input.mousePosition;

    public Vector2 CursorWorldPosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);
#elif ENABLE_INPUT_SYSTEM // 如果使用了新的输入系统
    public Vector2 CursorPosition => Mouse.current.position.ReadValue();

    public Vector2 CursorWorldPosition => Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
#endif

    public bool IsInitialized { get; set; }

    public void Init()
    {
        if (IsInitialized) return;

        try
        {
            _currentCursorDict = Enum.GetValues(typeof(CursorLayer))
                .Cast<CursorLayer>()
                .ToDictionary(cursorLayer => cursorLayer, _ => CursorID.None);
            SetCursor(CursorID.Default, CursorLayer.Base);
        }
        catch (Exception ex)
        {
            if (ex is CustomErrorException)
                throw;
            throw new CustomErrorException("[CursorManager] LoadTemplate failed",
                new CustomErrorItem(ErrorSeverity.Error, ErrorCode.CursorManagerInitFailed));
        }
        finally
        {
            IsInitialized = true;
        }
    }

    /// <summary>
    /// 设置光标
    /// </summary>
    /// <param name="targetCursor"> 光标ID </param>
    /// <param name="cursorLayer"> 光标层 </param>
    public void SetCursor(CursorID targetCursor, CursorLayer cursorLayer = CursorLayer.Temp)
    {
        if (_currentCursorDict[cursorLayer] == targetCursor)
            return;
        _currentCursorDict[cursorLayer] = targetCursor;


        _onCursorMgrUpdate -= UpdateCursorAnimation;

        _cursorFrame = 0;
        _frameTimer = CurrentCursor.FrameRate;
        Cursor.SetCursor(CurrentCursor.CursorTextures[0], CurrentCursor.CursorOffset, CursorMode.ForceSoftware);
        Cursor.visible = true;
        UpdateCursorAnimation();

        if (CurrentCursor.FrameCount > 1)
            _onCursorMgrUpdate += UpdateCursorAnimation;

    }

    /// <summary>
    /// 重置光标
    /// </summary>
    /// <param name="cursorLayer"> 光标层 </param>
    public void ResetCursor(CursorLayer cursorLayer = CursorLayer.Temp) => SetCursor(CursorID.None, cursorLayer);

    private void Update() => _onCursorMgrUpdate?.Invoke();

    /// <summary>
    /// 更新光标动画
    /// </summary>
    private void UpdateCursorAnimation()
    {
        _frameTimer -= Time.deltaTime;


        if (_frameTimer.CompareTo(0.0f) > 0) return;
        _cursorFrame = (_cursorFrame + 1) % CurrentCursor.FrameCount;
        _frameTimer = CurrentCursor.FrameRate;
        Cursor.SetCursor(CurrentCursor.CursorTextures[_cursorFrame], CurrentCursor.CursorOffset, CursorMode.ForceSoftware);
    }
}


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

#if ENABLE_LEGACY_INPUT_MANAGER // 旧输入管理器
    public Vector2 CursorPosition => Input.mousePosition;

    public Vector2 CursorWorldPosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);
#elif ENABLE_INPUT_SYSTEM // 新输入系统
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
    /// 设置指定层的光标
    /// </summary>
    /// <param name="targetCursor"> 指定的光标SO </param>
    /// <param name="cursorLayer"> 光标层级 </param>
    public void SetCursor(CursorID targetCursor, CursorLayer cursorLayer = CursorLayer.Temp)
    {
        if (_currentCursorDict[cursorLayer] == targetCursor)
            return;
        _currentCursorDict[cursorLayer] = targetCursor;

        // 处理动画更新：
        _onCursorMgrUpdate -= UpdateCursorAnimation;
        // 切换光标图标，并设置动画帧率
        _cursorFrame = 0;
        _frameTimer = CurrentCursor.FrameRate;
        Cursor.SetCursor(CurrentCursor.CursorTextures[0], CurrentCursor.CursorOffset, CursorMode.ForceSoftware);
        Cursor.visible = true;
        UpdateCursorAnimation();
        // 若光标有动画，则注册更新事件，否则只会在上一句更新图标
        if (CurrentCursor.FrameCount > 1)
            _onCursorMgrUpdate += UpdateCursorAnimation;
    }

    /// <summary>
    /// 重置指定层的光标
    /// </summary>
    /// <param name="cursorLayer"> 光标层级 </param>
    public void ResetCursor(CursorLayer cursorLayer = CursorLayer.Temp) => SetCursor(CursorID.None, cursorLayer);

    private void Update() => _onCursorMgrUpdate?.Invoke();

    /// <summary>
    /// 异步初始化光标字典,加载光标预加载资源
    /// </summary>
    /// <returns></returns>
    private async UniTask InitializeCursorDictionary()
    {
        _currentCursorDict = Enum.GetValues(typeof(CursorLayer))
            .Cast<CursorLayer>()
            .ToDictionary(layer => layer, _ => CursorID.None);

        // TODO: 可以在这里添加光标资源的预加载逻辑
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 更新光标动画
    /// </summary>
    private void UpdateCursorAnimation()
    {
        _frameTimer -= Time.deltaTime;

        // 如果光标动画播放完毕，切换到下一帧
        if (_frameTimer.CompareTo(0.0f) > 0) return;
        _cursorFrame = (_cursorFrame + 1) % CurrentCursor.FrameCount;
        _frameTimer = CurrentCursor.FrameRate;
        Cursor.SetCursor(CurrentCursor.CursorTextures[_cursorFrame], CurrentCursor.CursorOffset, CursorMode.ForceSoftware);
    }
}


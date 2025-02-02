/// <summary>
/// 错误严重性<br/>
/// 1：信息<br/>
/// 2：警告<br/>
/// 3：错误<br/>
/// 4：强制退出<br/>
/// </summary>
public enum ErrorSeverity
{
    Info = 1,
    Warning = 2,
    Error = 3,
    ForceQuit = 4
}

/// <summary>
/// 错误代码
/// </summary>
public enum ErrorCode
{
    GameInitFailed = 0,
    SceneCantLoad = 1,
    SceneOnLoadFailed = 2,
    UIInitFailed = 3,
    CanvasValidateFailed = 4,
    ComponentInitFailed = 5,
    UICanvasResetFailed = 6,

}

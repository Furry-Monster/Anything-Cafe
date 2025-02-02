/// <summary>
/// ����������<br/>
/// 1����Ϣ<br/>
/// 2������<br/>
/// 3������<br/>
/// 4��ǿ���˳�<br/>
/// </summary>
public enum ErrorSeverity
{
    Info = 1,
    Warning = 2,
    Error = 3,
    ForceQuit = 4
}

/// <summary>
/// �������
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

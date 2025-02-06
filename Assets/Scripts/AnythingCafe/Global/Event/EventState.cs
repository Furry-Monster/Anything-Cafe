/// <summary>
/// 游戏内事件的枚举,
/// 同时也是游戏事件的生命周期架构，每一个游戏事件必须可以经过下列所有周期
/// </summary>
public enum EventState
{
    NotActivated,
    Activated,
    Handling,
    Completed,
    Recorded
}

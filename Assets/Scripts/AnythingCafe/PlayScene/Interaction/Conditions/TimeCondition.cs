using System;
using UnityEngine;

public class TimeCondition : MonoBehaviour, ICondition
{
    [SerializeField] private int _startHour;
    [SerializeField] private int _endHour;
    [SerializeField] private bool _invertTime;

    public bool IsMet()
    {
        // 这里需要根据你的游戏时间系统实现具体的检查逻辑
        // 示例：检查当前游戏时间是否在指定范围内
        // int currentHour = GameTimeManager.Instance.CurrentHour;
        var currentHour = DateTime.Now.Hour; // 临时使用系统时间作为示例

        var isInRange = currentHour >= _startHour && currentHour < _endHour;
        return _invertTime ? !isInRange : isInRange;
    }

    public string GetFailureReason() => _invertTime
        ? $"只能在 {_endHour}:00 到 {_startHour}:00 之间交互"
        : $"只能在 {_startHour}:00 到 {_endHour}:00 之间交互";
}
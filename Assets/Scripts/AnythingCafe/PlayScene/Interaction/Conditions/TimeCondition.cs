using UnityEngine;
using System;

public class TimeCondition : MonoBehaviour, ICondition
{
    [SerializeField] private int startHour;
    [SerializeField] private int endHour;
    [SerializeField] private bool invertTime;
    
    public bool IsMet()
    {
        // 这里需要根据你的游戏时间系统实现具体的检查逻辑
        // 示例：检查当前游戏时间是否在指定范围内
        // int currentHour = GameTimeManager.Instance.CurrentHour;
        int currentHour = DateTime.Now.Hour; // 临时使用系统时间作为示例
        
        bool isInRange = currentHour >= startHour && currentHour < endHour;
        return invertTime ? !isInRange : isInRange;
    }
    
    public string GetFailureReason()
    {
        if (invertTime)
        {
            return $"只能在 {endHour}:00 到 {startHour}:00 之间交互";
        }
        return $"只能在 {startHour}:00 到 {endHour}:00 之间交互";
    }
} 
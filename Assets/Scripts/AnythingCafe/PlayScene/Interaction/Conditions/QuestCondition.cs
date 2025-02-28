using UnityEngine;

public class QuestCondition : MonoBehaviour, ICondition
{
    [SerializeField] private string questId;
    [SerializeField] private bool requireCompleted = true;
    
    public bool IsMet()
    {
        // 这里需要根据你的任务系统实现具体的检查逻辑
        // 示例：检查指定任务是否完成
        // return QuestManager.Instance.IsQuestCompleted(questId) == requireCompleted;
        return false;
    }
    
    public string GetFailureReason()
    {
        return requireCompleted ? 
            $"需要完成任务: {questId}" : 
            $"任务 {questId} 已完成，无法交互";
    }
} 
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Condition", menuName = "AnythingCafe/GamePlay/Conditions/Quest Condition")]
public class QuestCondition : ScriptableObject, ICondition
{
    [SerializeField] private string _questId;
    [SerializeField] private bool _requireCompleted = true;

    public bool IsMet()
    {
        // 这里需要根据你的任务系统实现具体的检查逻辑
        // 示例：检查指定任务是否完成
        // return QuestManager.Instance.IsQuestCompleted(questId) == requireCompleted;
        return false;
    }

    public string GetFailureReason() => _requireCompleted
            ? $"需要完成任务: {_questId}"
            : $"任务 {_questId} 已完成，无法交互";
}
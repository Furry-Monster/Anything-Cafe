using UnityEngine;

public class ItemCondition : MonoBehaviour, ICondition
{
    [SerializeField] private string itemId;
    [SerializeField] private int requiredAmount = 1;

    public bool IsMet()
    {
        // 这里需要根据你的物品系统实现具体的检查逻辑
        // 示例：检查玩家背包中是否有足够数量的指定物品
        // return InventoryManager.Instance.GetItemCount(itemId) >= requiredAmount;
        return false;
    }

    public string GetFailureReason()
    {
        return $"需要 {requiredAmount} 个 {itemId}";
    }
}
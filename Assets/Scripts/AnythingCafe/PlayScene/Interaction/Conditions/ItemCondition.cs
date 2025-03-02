using UnityEngine;

[CreateAssetMenu(fileName = "New Item Condition", menuName = "AnythingCafe/GamePlay/Conditions/Item Condition")]
public class ItemCondition : ScriptableObject, ICondition
{
    [SerializeField] private string _itemId;
    [SerializeField] private int _requiredAmount = 1;

    public bool IsMet()
    {
        // 这里需要根据你的物品系统实现具体的检查逻辑
        // 示例：检查玩家背包中是否有足够数量的指定物品
        // return InventoryManager.Instance.GetItemCount(itemId) >= requiredAmount;
        return false;
    }

    public string GetFailureReason() => $"需要 {_requiredAmount} 个 {_itemId}";
}
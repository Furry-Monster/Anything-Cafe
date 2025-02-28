using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConditionGroup : MonoBehaviour, ICondition
{
    [SerializeField] private List<MonoBehaviour> _conditions = new();
    [SerializeField] private bool _requireAllConditions = true;

    private List<ICondition> _validConditions = new();
    private string _lastFailureReason;

    private void Awake()
    {
        // 过滤出有效的条件组件
        foreach (var condition in _conditions)
        {
            if (condition is ICondition validCondition)
            {
                _validConditions.Add(validCondition);
            }
        }
    }

    public bool IsMet()
    {
        if (_validConditions.Count == 0) return true;

        if (_requireAllConditions)
        {
            // 所有条件都必须满足
            foreach (var condition in _validConditions.Where(condition => !condition.IsMet()))
            {
                _lastFailureReason = condition.GetFailureReason();
                return false;
            }

            return true;
        }
        else
        {
            // 只需要满足一个条件
            if (_validConditions.Any(condition => condition.IsMet()))
            {
                return true;
            }
            _lastFailureReason = "没有满足任何一个条件";
            return false;
        }
    }

    public string GetFailureReason()
    {
        return _lastFailureReason;
    }

    /// <summary>
    /// 添加一个条件
    /// </summary>
    /// <param name="condition"> 条件组件 </param>
    public void AddCondition(MonoBehaviour condition)
    {
        if (condition is ICondition validCondition)
        {
            _conditions.Add(condition);
            _validConditions.Add(validCondition);
        }
    }

    /// <summary>
    /// 移除一个条件
    /// </summary>
    /// <param name="condition"> 条件组件 </param>
    public void RemoveCondition(MonoBehaviour condition)
    {
        _conditions.Remove(condition);
        if (condition is ICondition validCondition)
        {
            _validConditions.Remove(validCondition);
        }
    }
}
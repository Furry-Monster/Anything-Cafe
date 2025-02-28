using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConditionGroup : MonoBehaviour, ICondition
{
    [SerializeField] private List<MonoBehaviour> conditions = new List<MonoBehaviour>();
    [SerializeField] private bool requireAllConditions = true;
    
    private List<ICondition> _validConditions = new List<ICondition>();
    private string _lastFailureReason;
    
    private void Awake()
    {
        // 过滤出有效的条件组件
        foreach (var condition in conditions)
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
        
        if (requireAllConditions)
        {
            // 所有条件都必须满足
            foreach (var condition in _validConditions)
            {
                if (!condition.IsMet())
                {
                    _lastFailureReason = condition.GetFailureReason();
                    return false;
                }
            }
            return true;
        }
        else
        {
            // 只需要满足一个条件
            foreach (var condition in _validConditions)
            {
                if (condition.IsMet())
                {
                    return true;
                }
            }
            _lastFailureReason = "没有满足任何一个条件";
            return false;
        }
    }
    
    public string GetFailureReason()
    {
        return _lastFailureReason;
    }
    
    public void AddCondition(MonoBehaviour condition)
    {
        if (condition is ICondition validCondition)
        {
            conditions.Add(condition);
            _validConditions.Add(validCondition);
        }
    }
    
    public void RemoveCondition(MonoBehaviour condition)
    {
        conditions.Remove(condition);
        if (condition is ICondition validCondition)
        {
            _validConditions.Remove(validCondition);
        }
    }
} 
using System;
using UnityEngine.Serialization;

/// <summary>
/// 序列化某个值,给值加上[Serializable]进行包装
/// </summary>
/// <typeparam name="TValue">需要被序列化的值的类型</typeparam>
[Serializable]
public class SerializeValue<TValue>
{
    [field: FormerlySerializedAs("value")] public TValue Value { get; set; }

    public SerializeValue(TValue value)
    {
        Value = value;
    }
}

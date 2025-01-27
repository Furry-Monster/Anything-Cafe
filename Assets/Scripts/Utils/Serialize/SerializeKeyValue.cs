using System;
using UnityEngine.Serialization;

/// <summary>
/// 序列化某个键值对,给其加上[Serializable]进行包装
/// </summary>
/// <typeparam name="TKey"> 需要被序列化的键</typeparam>
/// <typeparam name="TValue">需要被序列化的值</typeparam>
[Serializable]
public class SerializeKeyValue<TKey, TValue>
{
    [field: FormerlySerializedAs("Key")] public TKey Key { get; set; }
    [field: FormerlySerializedAs("Value")] public TValue Value { get; set; }

    public SerializeKeyValue(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}

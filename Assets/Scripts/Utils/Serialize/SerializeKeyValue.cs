using System;
using UnityEngine.Serialization;

/// <summary>
/// ���л�ĳ����ֵ��,�������[Serializable]���а�װ
/// </summary>
/// <typeparam name="TKey"> ��Ҫ�����л��ļ�</typeparam>
/// <typeparam name="TValue">��Ҫ�����л���ֵ</typeparam>
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

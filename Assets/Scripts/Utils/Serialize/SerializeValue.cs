using System;
using UnityEngine.Serialization;

/// <summary>
/// ���л�ĳ��ֵ,��ֵ����[Serializable]���а�װ
/// </summary>
/// <typeparam name="TValue">��Ҫ�����л���ֵ������</typeparam>
[Serializable]
public class SerializeValue<TValue>
{
    [field: FormerlySerializedAs("value")] public TValue Value { get; set; }

    public SerializeValue(TValue value)
    {
        Value = value;
    }
}

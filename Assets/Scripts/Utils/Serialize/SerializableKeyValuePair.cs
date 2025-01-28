using System;
using UnityEngine;
using Object = System.Object;

[Serializable]
public struct SerializableKeyValuePair<TKey,TValue>
{
    public TKey Key;
    public TValue Value;

    public SerializableKeyValuePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}
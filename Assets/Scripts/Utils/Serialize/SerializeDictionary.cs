//----------- 
// 三方代码
//
// 序列化字典的实现,来自：
// https://busyogg.github.io/article/e8fee5b5f443/
//-----------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SerializeDictionary { }

[Serializable]
public class SerializeDictionary<TKey, TValue> : SerializeDictionary, ISerializationCallbackReceiver, IDictionary<TKey, TValue>
{
    // 序列化的键值对列表，和下方KeyPositions表联动对应，用于查询
    [SerializeField] private List<SerializeKeyValue<TKey, TValue>> list = new();

    // 键的位置字典，使用Lazy初始化，记录了键在列表中的下标
    private Dictionary<TKey, int> KeyPositionDict => _keyPositionDict.Value;
    private Lazy<Dictionary<TKey, int>> _keyPositionDict;

    // 构造函数，初始化Lazy字典
    public SerializeDictionary()
    {
        _keyPositionDict = new Lazy<Dictionary<TKey, int>>(InitKeyPositionDict);
    }

    // 创建键的位置字典
    private Dictionary<TKey, int> InitKeyPositionDict()
    {
        var dictionary = new Dictionary<TKey, int>(list.Count);
        for (var i = 0; i < list.Count; i++)
        {
            dictionary[list[i].Key] = i;
        }
        return dictionary;
    }


    #region ISerializationCallbackReceiver 实现,提供Unity序列化回调
    // 序列化前的回调，不用管
    public void OnBeforeSerialize() { }

    // 反序列化后的回调，重新初始化Lazy字典
    public void OnAfterDeserialize()
    {
        _keyPositionDict = new Lazy<Dictionary<TKey, int>>(InitKeyPositionDict);
    }
    #endregion


    #region IDictionary<TKey, TValue> 实现,提供字典操作
    // 索引器，通过键获取或设置值
    public TValue this[TKey key]
    {
        get => list[KeyPositionDict[key]].Value; // 先查Key表，获取Key对应的int下标，再通过下标获取值
        set
        {
            var pair = new SerializeKeyValue<TKey, TValue>(key, value);
            if (KeyPositionDict.ContainsKey(key))
            {
                list[KeyPositionDict[key]] = pair;
            }
            else
            {
                KeyPositionDict[key] = list.Count;
                list.Add(pair);
            }
        }
    }

    // 获取所有键
    public ICollection<TKey> Keys =>
        list.Select(tuple => tuple.Key)
        .ToArray();
    // 获取所有值
    public ICollection<TValue> Values =>
        list.Select(tuple => tuple.Value)
            .ToArray();

    // 添加键值对
    public void Add(TKey key, TValue value)
    {
        // 检查是否存在相同的键
        if (KeyPositionDict.ContainsKey(key))
            throw new ArgumentException("An element with the same key already exists in the dictionary.");

        KeyPositionDict[key] = list.Count;
        list.Add(new SerializeKeyValue<TKey, TValue>(key, value));
    }

    // 检查是否包含指定键
    public bool ContainsKey(TKey key) => KeyPositionDict.ContainsKey(key);

    // 移除指定键的键值对
    public bool Remove(TKey key)
    {
        if (KeyPositionDict.TryGetValue(key, out var index))
        {
            KeyPositionDict.Remove(key);

            list.RemoveAt(index);
            // 调整KeyPositionDict的下标，较损耗性能！！！
            for (var i = index; i < list.Count; i++)
                KeyPositionDict[list[i].Key] = i;

            return true;
        }

        return false;
    }

    // 尝试获取指定键的值
    public bool TryGetValue(TKey key, out TValue value)
    {
        if (KeyPositionDict.TryGetValue(key, out var index))
        {
            value = list[index].Value;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }
    #endregion


    #region ICollection <KeyValuePair<TKey, TValue>> 实现,源自IDictionary<TKey, TValue>,提供集合操作
    // 获取键值对数量
    public int Count => list.Count;
    // 是否只读
    public bool IsReadOnly => false;

    // 添加键值对
    public void Add(KeyValuePair<TKey, TValue> kvp) => Add(kvp.Key, kvp.Value);
    // 清空所有键值对
    public void Clear() => list.Clear();
    // 检查是否包含指定键值对
    public bool Contains(KeyValuePair<TKey, TValue> kvp) => KeyPositionDict.ContainsKey(kvp.Key);

    // 复制键值对到数组
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        var numKeys = list.Count;
        if (array.Length - arrayIndex < numKeys)
            throw new ArgumentException($" Destination array is not long enough, array length is {array.Length}," +
                                        $" array index is {arrayIndex}, number of elements is {numKeys}");
        for (var i = 0; i < numKeys; i++, arrayIndex++)
        {
            var entry = list[i];
            array[arrayIndex] = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
        }
    }

    // 移除指定键值对
    public bool Remove(KeyValuePair<TKey, TValue> kvp) => Remove(kvp.Key);
    #endregion


    #region IEnumerable <KeyValuePair<TKey, TValue>> 实现,源自IDictionary<TKey, TValue>,提供枚举器
    // 获取枚举器，返回一个可以遍历字典中所有键值对的枚举器
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        // 将SerializeKeyValue转换为KeyValuePair，并获取其枚举器
        return list.Select(ToKeyValuePair).GetEnumerator();

        // 将SerializeKeyValue转换为KeyValuePair的方法
        static KeyValuePair<TKey, TValue> ToKeyValuePair(SerializeKeyValue<TKey, TValue> skvp)
        {
            return new KeyValuePair<TKey, TValue>(skvp.Key, skvp.Value);
        }
    }

    // 获取非泛型的枚举器，调用上面的泛型枚举器
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion


    // 非接口方法，去除序列化，转换为C#字典
    public Dictionary<TKey, TValue> ToDictionary()
    {
        var dic = new Dictionary<TKey, TValue>();
        foreach (var kvp in list)
        {
            dic.Add(kvp.Key, kvp.Value);
        }
        return dic;
    }
}

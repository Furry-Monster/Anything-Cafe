//----------- 
// 三方代码
//
// 序列化列表的实现,来自：
// https://busyogg.github.io/article/e8fee5b5f443/
//-----------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SerializeList<TValue> : ISerializationCallbackReceiver, IList<TValue>
{
    // 序列化的列表
    [SerializeField] private List<SerializeValue<TValue>> list = new();


    #region ISerializationCallbackReceiver 实现,提供Unity序列化回调
    // 序列化前的回调
    public void OnBeforeSerialize() { }

    // 反序列化后的回调
    public void OnAfterDeserialize() { }
    #endregion


    #region IEnumerable <KeyValuePair<TKey, TValue>> 实现 ,源自IList<TValue>,提供枚举器
    // 获取枚举器
    public IEnumerator<TValue> GetEnumerator()
    {
        return list.Select(ToKeyValuePair).GetEnumerator();

        static TValue ToKeyValuePair(SerializeValue<TValue> sb)
        {
            return sb.Value;
        }
    }

    // 获取非泛型枚举器
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion


    #region IList<TValue> 实现,提供列表操作
    // 索引器，用于获取或设置指定索引处的元素
    public TValue this[int index]
    {
        get => list[index].Value;
        set
        {
            if (index < list.Count)
            {
                SerializeValue<TValue> v = list[index];
                v.Value = value;
                list[index] = v;
            }
            else
            {
                list.Add(new SerializeValue<TValue>(value));
            }
        }
    }

    // 获取指定元素的索引
    public int IndexOf(TValue item)
    {
        for (var i = 0; i < list.Count; i++)
        {
            if (list[i].Value.Equals(item))
            {
                return i;
            }
        }
        return -1;
    }

    // 在指定索引处插入元素
    public void Insert(int index, TValue item)
    {
        list.Insert(index, new SerializeValue<TValue>(item));
    }

    // 移除指定索引处的元素
    public void RemoveAt(int index)
    {
        list.RemoveAt(index);
    }
    #endregion


    #region ICollection<TValue> 实现,提供集合操作
    // 列表中的元素数量
    public int Count => list.Count;

    // 是否只读
    public bool IsReadOnly => false;

    // 添加元素
    public void Add(TValue item) => list.Add(new SerializeValue<TValue>(item));
    // 清空列表
    public void Clear() => list.Clear();

    // 判断列表是否包含指定元素
    public bool Contains(TValue item)
    {
        var res = IndexOf(item);
        return res != -1;
    }

    // 将列表复制到数组中
    public void CopyTo(TValue[] array, int arrayIndex)
    {
        var numKeys = list.Count;
        if (array.Length - arrayIndex < numKeys)
            throw new ArgumentException("arrayIndex");
        for (var i = 0; i < numKeys; i++, arrayIndex++)
        {
            var entry = list[i];
            array[arrayIndex] = entry.Value;
        }
    }

    // 移除指定元素
    public bool Remove(TValue item)
    {
        var res = IndexOf(item);
        if (res != -1)
        {
            list.RemoveAt(res);
            return true;
        }
        return false;
    }
    #endregion


    // 非接口方法，去除序列化，转换为C#列表
    public List<TValue> ToList()
    {
        var l = new List<TValue>();
        foreach (var item in list)
        {
            l.Add(item.Value);
        }
        return l;
    }
}

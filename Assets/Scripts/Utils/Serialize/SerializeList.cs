//----------- 
// ��������
//
// ���л��б��ʵ��,���ԣ�
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
    // ���л����б�
    [SerializeField] private List<SerializeValue<TValue>> list = new();


    #region ISerializationCallbackReceiver ʵ��,�ṩUnity���л��ص�
    // ���л�ǰ�Ļص�
    public void OnBeforeSerialize() { }

    // �����л���Ļص�
    public void OnAfterDeserialize() { }
    #endregion


    #region IEnumerable <KeyValuePair<TKey, TValue>> ʵ�� ,Դ��IList<TValue>,�ṩö����
    // ��ȡö����
    public IEnumerator<TValue> GetEnumerator()
    {
        return list.Select(ToKeyValuePair).GetEnumerator();

        static TValue ToKeyValuePair(SerializeValue<TValue> sb)
        {
            return sb.Value;
        }
    }

    // ��ȡ�Ƿ���ö����
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion


    #region IList<TValue> ʵ��,�ṩ�б����
    // �����������ڻ�ȡ������ָ����������Ԫ��
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

    // ��ȡָ��Ԫ�ص�����
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

    // ��ָ������������Ԫ��
    public void Insert(int index, TValue item)
    {
        list.Insert(index, new SerializeValue<TValue>(item));
    }

    // �Ƴ�ָ����������Ԫ��
    public void RemoveAt(int index)
    {
        list.RemoveAt(index);
    }
    #endregion


    #region ICollection<TValue> ʵ��,�ṩ���ϲ���
    // �б��е�Ԫ������
    public int Count => list.Count;

    // �Ƿ�ֻ��
    public bool IsReadOnly => false;

    // ���Ԫ��
    public void Add(TValue item) => list.Add(new SerializeValue<TValue>(item));
    // ����б�
    public void Clear() => list.Clear();

    // �ж��б��Ƿ����ָ��Ԫ��
    public bool Contains(TValue item)
    {
        var res = IndexOf(item);
        return res != -1;
    }

    // ���б��Ƶ�������
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

    // �Ƴ�ָ��Ԫ��
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


    // �ǽӿڷ�����ȥ�����л���ת��ΪC#�б�
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

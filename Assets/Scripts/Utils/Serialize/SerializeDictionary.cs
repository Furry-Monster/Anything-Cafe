//----------- 
// ��������
//
// ���л��ֵ��ʵ��,���ԣ�
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
    // ���л��ļ�ֵ���б����·�KeyPositions��������Ӧ�����ڲ�ѯ
    [SerializeField] private List<SerializeKeyValue<TKey, TValue>> list = new();

    // ����λ���ֵ䣬ʹ��Lazy��ʼ������¼�˼����б��е��±�
    private Dictionary<TKey, int> KeyPositionDict => _keyPositionDict.Value;
    private Lazy<Dictionary<TKey, int>> _keyPositionDict;

    // ���캯������ʼ��Lazy�ֵ�
    public SerializeDictionary()
    {
        _keyPositionDict = new Lazy<Dictionary<TKey, int>>(InitKeyPositionDict);
    }

    // ��������λ���ֵ�
    private Dictionary<TKey, int> InitKeyPositionDict()
    {
        var dictionary = new Dictionary<TKey, int>(list.Count);
        for (var i = 0; i < list.Count; i++)
        {
            dictionary[list[i].Key] = i;
        }
        return dictionary;
    }


    #region ISerializationCallbackReceiver ʵ��,�ṩUnity���л��ص�
    // ���л�ǰ�Ļص������ù�
    public void OnBeforeSerialize() { }

    // �����л���Ļص������³�ʼ��Lazy�ֵ�
    public void OnAfterDeserialize()
    {
        _keyPositionDict = new Lazy<Dictionary<TKey, int>>(InitKeyPositionDict);
    }
    #endregion


    #region IDictionary<TKey, TValue> ʵ��,�ṩ�ֵ����
    // ��������ͨ������ȡ������ֵ
    public TValue this[TKey key]
    {
        get => list[KeyPositionDict[key]].Value; // �Ȳ�Key����ȡKey��Ӧ��int�±꣬��ͨ���±��ȡֵ
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

    // ��ȡ���м�
    public ICollection<TKey> Keys =>
        list.Select(tuple => tuple.Key)
        .ToArray();
    // ��ȡ����ֵ
    public ICollection<TValue> Values =>
        list.Select(tuple => tuple.Value)
            .ToArray();

    // ��Ӽ�ֵ��
    public void Add(TKey key, TValue value)
    {
        // ����Ƿ������ͬ�ļ�
        if (KeyPositionDict.ContainsKey(key))
            throw new ArgumentException("An element with the same key already exists in the dictionary.");

        KeyPositionDict[key] = list.Count;
        list.Add(new SerializeKeyValue<TKey, TValue>(key, value));
    }

    // ����Ƿ����ָ����
    public bool ContainsKey(TKey key) => KeyPositionDict.ContainsKey(key);

    // �Ƴ�ָ�����ļ�ֵ��
    public bool Remove(TKey key)
    {
        if (KeyPositionDict.TryGetValue(key, out var index))
        {
            KeyPositionDict.Remove(key);

            list.RemoveAt(index);
            // ����KeyPositionDict���±꣬��������ܣ�����
            for (var i = index; i < list.Count; i++)
                KeyPositionDict[list[i].Key] = i;

            return true;
        }

        return false;
    }

    // ���Ի�ȡָ������ֵ
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


    #region ICollection <KeyValuePair<TKey, TValue>> ʵ��,Դ��IDictionary<TKey, TValue>,�ṩ���ϲ���
    // ��ȡ��ֵ������
    public int Count => list.Count;
    // �Ƿ�ֻ��
    public bool IsReadOnly => false;

    // ��Ӽ�ֵ��
    public void Add(KeyValuePair<TKey, TValue> kvp) => Add(kvp.Key, kvp.Value);
    // ������м�ֵ��
    public void Clear() => list.Clear();
    // ����Ƿ����ָ����ֵ��
    public bool Contains(KeyValuePair<TKey, TValue> kvp) => KeyPositionDict.ContainsKey(kvp.Key);

    // ���Ƽ�ֵ�Ե�����
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

    // �Ƴ�ָ����ֵ��
    public bool Remove(KeyValuePair<TKey, TValue> kvp) => Remove(kvp.Key);
    #endregion


    #region IEnumerable <KeyValuePair<TKey, TValue>> ʵ��,Դ��IDictionary<TKey, TValue>,�ṩö����
    // ��ȡö����������һ�����Ա����ֵ������м�ֵ�Ե�ö����
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        // ��SerializeKeyValueת��ΪKeyValuePair������ȡ��ö����
        return list.Select(ToKeyValuePair).GetEnumerator();

        // ��SerializeKeyValueת��ΪKeyValuePair�ķ���
        static KeyValuePair<TKey, TValue> ToKeyValuePair(SerializeKeyValue<TKey, TValue> skvp)
        {
            return new KeyValuePair<TKey, TValue>(skvp.Key, skvp.Value);
        }
    }

    // ��ȡ�Ƿ��͵�ö��������������ķ���ö����
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion


    // �ǽӿڷ�����ȥ�����л���ת��ΪC#�ֵ�
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

using UnityEditor;
using UnityEngine;

/// <summary>
/// <para>SerializeDictionary ���Զ�������Drawer����</para>
/// <para> �����ٱ༭������ʾSerializeDictionary���б�����</para>
/// </summary>
[CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
public class SerializableDictionaryDrawer : PropertyDrawer
{
    // �����SerializedProperty�����ڴ洢�ֵ���б�����
    private SerializedProperty _listProperty;

    // ��ȡ�ֵ���б����ԣ�����ѻ�����ֱ�ӷ���
    private SerializedProperty DOGetListProperty(SerializedProperty property) =>
        _listProperty ??= property.FindPropertyRelative("list");

    // ���������ֶε�GUI
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, DOGetListProperty(property), label, true);
    }

    // ��ȡ�����ֶεĸ߶�
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(DOGetListProperty(property), true);
    }
}

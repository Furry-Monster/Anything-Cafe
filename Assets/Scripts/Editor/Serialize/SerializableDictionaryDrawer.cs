using UnityEditor;
using UnityEngine;

/// <summary>
/// <para>SerializeDictionary 的自定义属性Drawer绘制</para>
/// <para> 用于再编辑器中显示SerializeDictionary的列表属性</para>
/// </summary>
[CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
public class SerializableDictionaryDrawer : PropertyDrawer
{
    // 缓存的SerializedProperty，用于存储字典的列表属性
    private SerializedProperty _listProperty;

    // 获取字典的列表属性，如果已缓存则直接返回
    private SerializedProperty DOGetListProperty(SerializedProperty property) =>
        _listProperty ??= property.FindPropertyRelative("list");

    // 绘制属性字段的GUI
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, DOGetListProperty(property), label, true);
    }

    // 获取属性字段的高度
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(DOGetListProperty(property), true);
    }
}

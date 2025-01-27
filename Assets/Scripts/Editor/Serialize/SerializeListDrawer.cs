using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SerializeList<>))]
public class SerializeListDrawer : PropertyDrawer
{
    // 缓存的SerializedProperty，用于存储列表属性
    private SerializedProperty _listProperty;

    // 获取列表属性，如果已缓存则直接返回
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

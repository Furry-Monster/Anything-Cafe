using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[CustomEditor(typeof(Button))]
public class ButtonSoundEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var btn = (Button)target;

        // ֻ��δ�����Ч���ʱ��ʾ��ť
        if (!btn.TryGetComponent<ButtonSound>(out _))
        {
            if (GUILayout.Button("Add ButtonSound"))
            {
                if (btn.gameObject.TryGetComponent<ButtonSound>(out _) == false)
                    btn.gameObject.AddComponent<ButtonSound>();

                if (btn.gameObject.TryGetComponent<EventTrigger>(out _) == false)
                    btn.gameObject.AddComponent<EventTrigger>();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Sound component already added", MessageType.Info);
        }
    }
}

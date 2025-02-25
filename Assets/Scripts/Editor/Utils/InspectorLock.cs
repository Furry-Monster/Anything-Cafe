using UnityEditor;
using UnityEngine;

public class InspectorLock
{
    [MenuItem("GameObject/InspectorLock/Lock", false, 0)]
    private static void Lock() => SetHideFlags(HideFlags.NotEditable);

    [MenuItem("GameObject/InspectorLock/UnLock", false, 1)]
    private static void UnLock() => SetHideFlags(HideFlags.None);

    // Ëø¶¨Inspector
    private static void SetHideFlags(HideFlags hideFlags)
    {
        if (Selection.gameObjects != null)
        {
            foreach (var gameObject in Selection.gameObjects)
            {
                gameObject.hideFlags = hideFlags;
                EditorUtility.SetDirty(gameObject);
            }
        }
    }
}

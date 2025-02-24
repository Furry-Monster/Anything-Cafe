using UnityEngine;

[CreateAssetMenu(fileName = "MSDefaultSettings", menuName = "MonStore/Default Settings")]
public class MSDefaultSettings : ScriptableObject
{
    public MSSettings DefaultSettings;

    // Settings below can't be modified during runtime.
    public static bool LogInfo;
    public static bool LogWarning = true;
    public static bool LogError = true;

    private void OnEnable()
    {
        DefaultSettings = new MSSettings(false);
    }

    private void OnDisable()
    {
        DefaultSettings = null;
    }
}

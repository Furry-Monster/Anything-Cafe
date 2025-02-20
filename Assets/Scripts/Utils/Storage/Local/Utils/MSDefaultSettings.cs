using UnityEngine;


public class MSDefaultSettings : ScriptableObject
{
    public MSSerializableSettings SerializableSettings = new();
    public bool AutoAddMgrToScene;
    public bool LogInfo;
    public bool LogWarning = true;
    public bool LogError = true;
}

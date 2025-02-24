using UnityEngine;

internal class MSDebugger
{
    private static string Indent(int size) => size <= 0 ? "" : new string('-', size);

    public static void Log(string msg, Object context = null, int indent = 0)
    {
        if (MSDefaultSettings.LogInfo)
            Debug.LogFormat(context, Indent(indent) + msg);
    }

    public static void LogWarning(string msg, Object context = null, int indent = 0)
    {
        if (MSDefaultSettings.LogWarning)
            Debug.LogWarningFormat(context, Indent(indent) + msg);
    }

    public static void LogError(string msg, Object context = null, int indent = 0)
    {
        if (MSDefaultSettings.LogError)
            Debug.LogErrorFormat(context, Indent(indent) + msg);
    }
}

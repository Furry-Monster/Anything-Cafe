using UnityEngine;

public class MSDebugger
{
    private static string Indent(int size) => size <= 0 ? "" : new string('-', size);

    public static void Log(string msg, Object context = null, int indent = 0)
    {
        if (!MSSettings.DefaultSettings.LogInfo)
            return;

        Debug.LogFormat(context, Indent(indent) + msg);
    }

    public static void LogWarning(string msg, Object context = null, int indent = 0)
    {
        if (!MSSettings.DefaultSettings.LogWarning)
            return;

        Debug.LogWarningFormat(context, Indent(indent) + msg);
    }

    public static void LogError(string msg, Object context = null, int indent = 0)
    {
        if (!MSSettings.DefaultSettings.LogError)
            return;

        Debug.LogErrorFormat(context, Indent(indent) + msg);
    }
}

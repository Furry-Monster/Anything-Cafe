using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonStore
{
    #region SaveAPI ±£´æ
    public static void Save(string key, object value) => Save<object>(key, value, new MSSettings((string)null, (MSSettings)null));

    public static void Save(string key, object value, string filePath) => Save<object>(key, value, new MSSettings(filePath, (MSSettings)null));

    public static void Save(string key, object value, string filePath, MSSettings settings) => Save<object>(key, value, new MSSettings(filePath, settings));

    public static void Save(string key, object value, MSSettings settings) => Save<object>(key, value, settings);

    public static void Save<T>(string key, T value) => Save<T>(key, value, new MSSettings((string)null, (MSSettings)null));

    public static void Save<T>(string key, T value, string filePath) => Save<T>(key, value, new MSSettings(filePath, (MSSettings)null));

    public static void Save<T>(string key, T value, string filePath, MSSettings settings) => Save<T>(key, value, new MSSettings(filePath, settings));

    public static void Save<T>(string key, T value, MSSettings settings)
    {
        // TODO: Implement saving logic here
    }
    #endregion

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonStore
{
    public static void Save(string key, object value) => Save<object>(key, value);

    public static void Save(string key, object value, string filePath) => Save<object>(key, value);

    public static void Save<T>(string key, T value)
    {

    }
}

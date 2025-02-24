using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MSTypeManager
{
    private static readonly object Lock = new();
    private static Dictionary<Type, MSType> _types = null;

    public static void RegisterType(Type type, MSType msType)
    {
        if (_types == null) Init();

        var existingType = _types.GetValueOrDefault(type, null);
        if (existingType != null) return;

        lock (Lock)
        {
            _types![type] = msType;
        }
    }

    public static MSType GetMSType(Type type)
    {
        if (_types == null) Init();
        return _types.GetValueOrDefault(type, null);
    }

    internal static MSType AutoRegister(Type type)
    {
        // TODO: 
        throw new NotImplementedException();
    }

    internal static void Init()
    {
        lock (Lock)
        {
            _types = new Dictionary<Type, MSType>();
            // TODO: 通过反射自动获取所有类型并注册
            if (_types?.Count == 0)
                throw new TypeLoadException(
                    "Failed to load all types in the Assembly-CSharp.dll, " +
                    "which is required for the MSTypeManager to work. ");
        }
    }
}

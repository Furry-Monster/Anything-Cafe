using System;
using System.Linq;
using System.Reflection;

internal static class MSReflection
{
    private static Assembly[] _assemblies;

    public static Assembly[] Assemblies
    {
        get
        {
            if (_assemblies == null)
            {
                var assemblyNames = new MSSettings((string)null).AssemblyNames;

                _assemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(assembly => assemblyNames.Contains(assembly.GetName().Name))
                    .ToArray();
            }

            return _assemblies;
        }
    }

    public static bool IsValueType(Type type) => type.IsValueType;

    public static bool IsGenericType(Type type) => type.IsGenericType;

    public static bool IsAbstract(Type type) => type.IsAbstract;

    public static bool IsInterface(Type type) => type.IsInterface;

    public static bool IsEnum(Type type) => type.IsEnum;
}

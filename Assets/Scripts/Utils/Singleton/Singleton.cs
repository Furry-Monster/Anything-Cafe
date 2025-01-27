using System;

/// <summary>
/// 懒加载常规单例
/// </summary>
/// <typeparam name="T"> 需要变成单例的类 </typeparam>
public class Singleton<T> where T : class, new()
{
    // 使用Lazy<T>来实现单例
    // Lazy<T>是一个懒加载的泛型类，只有在第一次访问Instance属性时才会创建实例，并且保证线程安全
    private static readonly Lazy<T> _instance = new Lazy<T>(() => new T());

    public static T Instance => _instance.Value;

    protected Singleton()
    {
        if (_instance.IsValueCreated)
        {
            throw new Exception($"{typeof(T)} is a singleton! " +
                                "You should not create a new instance!" +
                                $" Use {typeof(T)}.Instance instead."
                                );
        }
    }

    // 这个地方原本打算写个GC机制来自动回收
    //
    //
    //
    // 考虑到性能问题，暂时放弃
}

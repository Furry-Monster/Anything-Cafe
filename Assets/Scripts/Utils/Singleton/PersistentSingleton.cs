using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : PersistentSingleton<T>
{
    private static T _instance;
    private static readonly object _lock = new object(); // 线程锁
    private static bool _applicationIsQuitting = false; // 标记是否退出游戏

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                // 退出游戏时，返回null,同时打印日志，防止出现
                // 【退出游戏瞬间的访问空引用，又没有错误日志打印】 的情况
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed on application quit. Won't create again - returning null.");
                return null;
            }

            // 锁定线程，防止多线程访问
            lock (_lock)
            {
                // 上锁后，再次判断_instance是否为空，防止多线程访问时，_instance已经被其他线程设置为空
                if (_instance == null)
                {
                    // 如果_instance为空，则尝试查找该脚本的实例
                    _instance = FindObjectOfType<T>();

                    if (FindObjectsOfType<T>().Length > 1)
                    {
                        Debug.LogError($"[Singleton] Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
                        return _instance;
                    }

                    // 如果找不到该脚本的实例
                    // 则创建一个新的GameObject，添加该脚本的组件，并设置为DontDestroyOnLoad
                    if (_instance == null)
                    {
                        var singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T).ToString();

                        DontDestroyOnLoad(singleton);
#if UNITY_EDITOR
                        Debug.Log($"[Singleton] An instance of {typeof(T)} is needed in the scene, so '{singleton}' was created with DontDestroyOnLoad.");
#endif
                    }
                    else
                    {
#if UNITY_EDITOR
                        Debug.Log($"[Singleton] Using instance already created: {_instance.gameObject.name}");
#endif
                    }
                }

                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            // 如果_instance引用为空，则指向该实例
            _instance = this as T;
            DontDestroyOnLoad(this.gameObject); // 在场景之间持久化单例
        }
        else if (_instance != this)
        {
            // 如果_instance引用有问题，摧毁当前实例，不进行覆盖
            Debug.LogWarning($"Multiple instances of {typeof(T)} found. Destroying this one.");
            Destroy(this.gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        // 当游戏退出时，将_instance置为null
        // 防止其他脚本访问已经销毁的对象，而又没有错误日志打印
        if (_instance == this)
        {
            _instance = null;
#if UNITY_EDITOR
            Debug.Log($"[Singleton] Destroying {typeof(T)} Singleton.");
#endif
        }
    }

    private void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }
}

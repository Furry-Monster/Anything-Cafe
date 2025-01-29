using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : PersistentSingleton<T>
{
    private static T _instance;
    private static readonly object _lock = new object(); // �߳���
    private static bool _applicationIsQuitting = false; // ����Ƿ��˳���Ϸ

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                // �˳���Ϸʱ������null,ͬʱ��ӡ��־����ֹ����
                // ���˳���Ϸ˲��ķ��ʿ����ã���û�д�����־��ӡ�� �����
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed on application quit. Won't create again - returning null.");
                return null;
            }

            // �����̣߳���ֹ���̷߳���
            lock (_lock)
            {
                // �������ٴ��ж�_instance�Ƿ�Ϊ�գ���ֹ���̷߳���ʱ��_instance�Ѿ��������߳�����Ϊ��
                if (_instance == null)
                {
                    // ���_instanceΪ�գ����Բ��Ҹýű���ʵ��
                    _instance = FindObjectOfType<T>();

                    if (FindObjectsOfType<T>().Length > 1)
                    {
                        Debug.LogError($"[Singleton] Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
                        return _instance;
                    }

                    // ����Ҳ����ýű���ʵ��
                    // �򴴽�һ���µ�GameObject����Ӹýű��������������ΪDontDestroyOnLoad
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
            // ���_instance����Ϊ�գ���ָ���ʵ��
            _instance = this as T;
            DontDestroyOnLoad(this.gameObject); // �ڳ���֮��־û�����
        }
        else if (_instance != this)
        {
            // ���_instance���������⣬�ݻٵ�ǰʵ���������и���
            Debug.LogWarning($"Multiple instances of {typeof(T)} found. Destroying this one.");
            Destroy(this.gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        // ����Ϸ�˳�ʱ����_instance��Ϊnull
        // ��ֹ�����ű������Ѿ����ٵĶ��󣬶���û�д�����־��ӡ
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

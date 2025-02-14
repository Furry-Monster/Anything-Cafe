using UnityEngine;

namespace FrameMonster.IOC
{

    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
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
                    Debug.LogWarning(
                        $"[MonoSingleton] Instance '{typeof(T)}' already destroyed on application quit. Won't create again - returning null.");
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
                            Debug.LogError(
                                $"[MonoSingleton] Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
                            return _instance;
                        }

                        // ����Ҳ����ýű���ʵ��
                        // �򴴽�һ���µ�GameObject����Ӹýű������
                        if (_instance == null)
                        {
                            var singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = typeof(T).ToString();
#if UNITY_EDITOR
                            Debug.Log(
                                $"[MonoSingleton] An instance of {typeof(T)} is needed in the scene, so '{singleton}' was created with DontDestroyOnLoad.");
#endif
                        }
                        else
                        {
#if UNITY_EDITOR
                            Debug.Log($"[MonoSingleton] Using instance already created: {_instance.gameObject.name}");
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
            }
            else if (_instance != this)
            {
                // ���_instance���������⣬�ݻٵ�ǰʵ���������и���
                Debug.LogWarning($"[MonoSingleton] Multiple instances of {typeof(T)} found. Destroying this one.");
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
                Debug.Log($"[MonoSingleton] Destroying {typeof(T)} Singleton.");
#endif
            }
        }

        private void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }
    }
}
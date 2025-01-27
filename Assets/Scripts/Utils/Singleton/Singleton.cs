using System;

/// <summary>
/// �����س��浥��
/// </summary>
/// <typeparam name="T"> ��Ҫ��ɵ������� </typeparam>
public class Singleton<T> where T : class, new()
{
    // ʹ��Lazy<T>��ʵ�ֵ���
    // Lazy<T>��һ�������صķ����ֻ࣬���ڵ�һ�η���Instance����ʱ�Żᴴ��ʵ�������ұ�֤�̰߳�ȫ
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

    // ����ط�ԭ������д��GC�������Զ�����
    //
    //
    //
    // ���ǵ��������⣬��ʱ����
}

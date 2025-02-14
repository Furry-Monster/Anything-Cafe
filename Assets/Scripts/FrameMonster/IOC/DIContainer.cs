using System;
using System.Collections.Generic;

namespace FrameMonster.IOC
{
    public class DIContainer : IContainer
    {
        private readonly Dictionary<Type, object> _registry = new();
        private readonly Dictionary<string, object> _namedRegistry = new();

        public void Register<TInterface, TImpl>(string id, bool isSingleton)
            where TImpl : TInterface
        {
            // ��ȡ���캯��
            Func<object> constructor = () => Activator.CreateInstance<TImpl>();

            // ע������
            _registry[typeof(TInterface)] =
                isSingleton ? constructor.Invoke() : constructor;

            // ע�����
            if (id != null) _namedRegistry[id] = _registry[typeof(TInterface)];
            else _namedRegistry[typeof(TInterface).Name] = _registry[typeof(TInterface)];
        }

        public TInterface Resolve<TInterface>()
        {
            if (_registry.TryGetValue(typeof(TInterface), out var instance))
            {
                if (instance is Func<object> constructor)
                {
                    return (TInterface)constructor.Invoke();
                }
                return (TInterface)instance;
            }
            throw new InvalidOperationException($"δע������ {typeof(TInterface)}");
        }

        public TInterface Resolve<TInterface>(string id)
        {
            if (_namedRegistry.TryGetValue(id, out var instance))
            {
                if (instance is Func<object> constructor)
                {
                    return (TInterface)constructor.Invoke();
                }
                return (TInterface)instance;
            }
            throw new InvalidOperationException($"δע��IDΪ {id} ������ {typeof(TInterface)}");
        }

        public void UnregisterAll()
        {
            _registry.Clear();
            _namedRegistry.Clear();
        }

    }
}
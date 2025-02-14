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
            // 获取构造函数
            Func<object> constructor = () => Activator.CreateInstance<TImpl>();

            // 注册依赖
            _registry[typeof(TInterface)] =
                isSingleton ? constructor.Invoke() : constructor;

            // 注册别名
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
            throw new InvalidOperationException($"未注册类型 {typeof(TInterface)}");
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
            throw new InvalidOperationException($"未注册ID为 {id} 的类型 {typeof(TInterface)}");
        }

        public void UnregisterAll()
        {
            _registry.Clear();
            _namedRegistry.Clear();
        }

    }
}
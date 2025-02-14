using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameMonster.IOC
{
    public class EasyContainer : IContainer
    {
        private readonly Dictionary<Type, object> _registry = new();

        public void Register<TInterface, TImpl>(string id, bool isSingleton = true)
            where TImpl : TInterface
        {
            if (id != null) throw new InvalidOperationException("EasyContainer doesn't support named registration.");

            Func<object> constructor = () => Activator.CreateInstance<TImpl>();

            if (isSingleton)
            {
                _registry[typeof(TInterface)] = constructor.Invoke();
            }
            else
            {
                throw new InvalidOperationException("EasyContainer doesn't support non-singleton registration.");
            }
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
            throw new InvalidOperationException($"Œ¥◊¢≤·¿‡–Õ {typeof(TInterface)}");
        }

        public void UnregisterAll()
        {
            _registry.Clear();
        }
    }
}

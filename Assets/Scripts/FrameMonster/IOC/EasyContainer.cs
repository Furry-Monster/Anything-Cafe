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
                Debug.LogError("[IoC] EasyContainer doesn't support non-singleton registration.");
            }
        }

        public void UnregisterAll()
        {
            _registry.Clear();
        }
    }
}

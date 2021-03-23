using System;
using System.Collections.Generic;
using System.Linq;

namespace IoCContainer.Core
{
    public class MyAdvancedContainer
    {
        readonly Dictionary<Type, Func<object>> _registrations = new Dictionary<Type, Func<object>>();

        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _registrations.Add(typeof(TInterface), () => GetInstance(typeof(TImplementation)));
        }

        public void Register<TService>(Func<TService> instanceCreator)
        {
            _registrations.Add(typeof(TService), () => instanceCreator());
        }

        public void RegisterSingleton<TService>(TService instance)
        {
            _registrations.Add(typeof(TService), () => instance);
        }

        public void RegisterSingleton<TService>(Func<TService> instanceCreator)
        {
            var lazy = new Lazy<TService>(instanceCreator);
            Register<TService>(() => lazy.Value);
        }

        public object GetInstance(Type serviceType)
        {
            Func<object> creator;

            if (_registrations.TryGetValue(serviceType, out creator))
            { 
                return creator();
            }

            if (!serviceType.IsAbstract)
            {
                return CreateInstance(serviceType);
            }

            throw new InvalidOperationException("No registration for " + serviceType);
        }

        private object CreateInstance(Type implementationType)
        {
            var ctor = implementationType.GetConstructors().Single();
            var parameterTypes = ctor.GetParameters().Select(p => p.ParameterType);
            var dependencies = parameterTypes.Select(t => this.GetInstance(t)).ToArray();

            return Activator.CreateInstance(implementationType, dependencies);
        }
    }
}
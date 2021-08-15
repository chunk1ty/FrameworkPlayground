using System;
using System.Collections.Generic;
using System.Linq;

namespace DiContainer.Core
{
    public class MyAdvancedContainer
    {
        private readonly Dictionary<Type, ServiceDescriptor> _serviceDescriptors = new Dictionary<Type, ServiceDescriptor>();

        public void RegisterTransient<TService, TImplementation>()
            where TImplementation : TService
        {
            _serviceDescriptors.Add(typeof(TService), new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient));
        }

        public void RegisterTransient<TService>()
        {
            _serviceDescriptors.Add(typeof(TService), new ServiceDescriptor(typeof(TService), typeof(TService), ServiceLifetime.Transient));
        }

        public void RegisterTransient<TService>(TService instance)
        {
            _serviceDescriptors.Add(typeof(TService), new ServiceDescriptor(typeof(TService), instance, ServiceLifetime.Transient));
        }

        public void RegisterSingleton<TService, TImplementation>()
            where TImplementation : TService
        {
            _serviceDescriptors.Add(typeof(TService), new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton));
        }

        public void RegisterSingleton<TService>()
        {
            _serviceDescriptors.Add(typeof(TService), new ServiceDescriptor(typeof(TService), typeof(TService), ServiceLifetime.Singleton));
        }

        public void RegisterSingleton<TService>(TService instance)
        {
            _serviceDescriptors.Add(typeof(TService), new ServiceDescriptor(typeof(TService), instance, ServiceLifetime.Singleton));
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        private object GetService(Type serviceType)
        {
            if (!_serviceDescriptors.ContainsKey(serviceType))
            {
                throw new InvalidOperationException($"No registration for: [{serviceType}].");
            }

            ServiceDescriptor serviceDescriptor = _serviceDescriptors[serviceType];

            if (serviceDescriptor.ImplementationType.IsAbstract || serviceDescriptor.ImplementationType.IsInterface)
            {
                throw new InvalidOperationException($"Cannot instantiate abstract class or interface: [{serviceDescriptor.ImplementationType}].");
            }

            if (serviceDescriptor.Implementation != null)
            {
                return serviceDescriptor.Implementation;
            }

            object implementationInstance = CreateNewInstance(serviceDescriptor.ImplementationType);

            if (serviceDescriptor.Lifetime == ServiceLifetime.Singleton)
            {
                serviceDescriptor.Implementation = implementationInstance;
            }

            return implementationInstance;
        }

        private object CreateNewInstance(Type implementationType)
        {
            var ctor = implementationType.GetConstructors().First();
            var dependencies = ctor.GetParameters().Select(p => GetService(p.ParameterType)).ToArray();

            return Activator.CreateInstance(implementationType, dependencies);
        }
    }

    public enum ServiceLifetime
    {
        Singleton,
        Transient,
    }

    public class ServiceDescriptor
    {
        public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
        }

        public ServiceDescriptor(Type serviceType, object implementation, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            ImplementationType = implementation.GetType();
            Implementation = implementation;
            Lifetime = lifetime;
        }

        public Type ServiceType { get; }

        public Type ImplementationType { get; }

        // keeps register service implementation instance.
        public object Implementation { get; set; }

        public ServiceLifetime Lifetime { get; }
    }
}
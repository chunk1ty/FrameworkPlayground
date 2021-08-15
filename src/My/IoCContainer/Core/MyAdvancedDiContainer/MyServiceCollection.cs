using System;
using System.Collections.Generic;

namespace DiContainer.Core.MyAdvancedDiContainer
{
    public class MyServiceCollection
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

        public MyDiContainer GenerateContainer()
        {
            return new MyDiContainer(_serviceDescriptors);
        }
    }
}
using System;
using System.Collections.Generic;

namespace DiContainer.Core.MyAdvancedDiContainer
{
    public class MyServiceCollection
    {
        private readonly Dictionary<Type, MyServiceDescriptor> _serviceDescriptors = new Dictionary<Type, MyServiceDescriptor>();

        public void RegisterTransient<TService, TImplementation>()
            where TImplementation : TService
        {
            _serviceDescriptors.Add(typeof(TService), new MyServiceDescriptor(typeof(TService), typeof(TImplementation), MyServiceLifetime.Transient));
        }

        public void RegisterTransient<TService>()
        {
            _serviceDescriptors.Add(typeof(TService), new MyServiceDescriptor(typeof(TService), typeof(TService), MyServiceLifetime.Transient));
        }

        public void RegisterTransient<TService>(TService instance)
        {
            _serviceDescriptors.Add(typeof(TService), new MyServiceDescriptor(typeof(TService), instance, MyServiceLifetime.Transient));
        }

        public void RegisterSingleton<TService, TImplementation>()
            where TImplementation : TService
        {
            _serviceDescriptors.Add(typeof(TService), new MyServiceDescriptor(typeof(TService), typeof(TImplementation), MyServiceLifetime.Singleton));
        }

        public void RegisterSingleton<TService>()
        {
            _serviceDescriptors.Add(typeof(TService), new MyServiceDescriptor(typeof(TService), typeof(TService), MyServiceLifetime.Singleton));
        }

        public void RegisterSingleton<TService>(TService instance)
        {
            _serviceDescriptors.Add(typeof(TService), new MyServiceDescriptor(typeof(TService), instance, MyServiceLifetime.Singleton));
        }

        public void RegisterSingleton<TService>(Func<IServiceProvider, object> implementationFactory)
        {
            _serviceDescriptors.Add(typeof(TService), new MyServiceDescriptor(typeof(TService), implementationFactory, MyServiceLifetime.Singleton));
        }

        public void AddServiceDescriptor(IEnumerable<MyServiceDescriptor> serviceDescriptors)
        {
            foreach (var serviceDescriptor in serviceDescriptors)
            {
                _serviceDescriptors.Add(serviceDescriptor.ServiceType, serviceDescriptor);
            }
        }

        public MyDiContainer BuildContainer()
        {
            return new MyDiContainer(_serviceDescriptors);
        }
    }
}
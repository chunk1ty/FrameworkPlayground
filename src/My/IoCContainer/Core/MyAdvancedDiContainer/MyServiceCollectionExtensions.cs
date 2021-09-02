using System;
using System.Collections.Generic;

namespace DiContainer.Core.MyAdvancedDiContainer
{
    public static class MyServiceCollectionExtensions
    {
        private static Dictionary<Type, MyServiceDescriptor> _serviceDescriptors = new Dictionary<Type, MyServiceDescriptor>();


        public static void RegisterTransient<TService, TImplementation>()
            where TImplementation : TService
        {
            _serviceDescriptors.Add(typeof(TService), new MyServiceDescriptor(typeof(TService), typeof(TImplementation), MyServiceLifetime.Transient));
        }

        public static void RegisterTransient<TService>()
        {
            _serviceDescriptors.Add(typeof(TService), new MyServiceDescriptor(typeof(TService), typeof(TService), MyServiceLifetime.Transient));
        }

        public static void RegisterTransient<TService>(TService instance)
        {
            _serviceDescriptors.Add(typeof(TService), new MyServiceDescriptor(typeof(TService), instance, MyServiceLifetime.Transient));
        }

        public static void RegisterSingleton<TService, TImplementation>()
            where TImplementation : TService
        {
            _serviceDescriptors.Add(typeof(TService), new MyServiceDescriptor(typeof(TService), typeof(TImplementation), MyServiceLifetime.Singleton));
        }

        public static void RegisterSingleton<TService>()
        {
            _serviceDescriptors.Add(typeof(TService), new MyServiceDescriptor(typeof(TService), typeof(TService), MyServiceLifetime.Singleton));
        }

        public static void RegisterSingleton<TService>(TService instance)
        {
            _serviceDescriptors.Add(typeof(TService), new MyServiceDescriptor(typeof(TService), instance, MyServiceLifetime.Singleton));
        }

        public static MyDiContainer GenerateContainer()
        {
            return new MyDiContainer(_serviceDescriptors);
        }
    }
}
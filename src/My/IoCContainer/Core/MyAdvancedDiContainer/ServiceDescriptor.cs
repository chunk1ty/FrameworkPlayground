using System;

namespace DiContainer.Core.MyAdvancedDiContainer
{
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
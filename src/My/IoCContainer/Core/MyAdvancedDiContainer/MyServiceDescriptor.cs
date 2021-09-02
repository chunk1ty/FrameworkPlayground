using System;

namespace DiContainer.Core.MyAdvancedDiContainer
{
    public class MyServiceDescriptor
    {
        public MyServiceDescriptor(Type serviceType, Type implementationType, MyServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
        }

        public MyServiceDescriptor(Type serviceType, object implementation, MyServiceLifetime lifetime)
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

        public MyServiceLifetime Lifetime { get; }
    }
}
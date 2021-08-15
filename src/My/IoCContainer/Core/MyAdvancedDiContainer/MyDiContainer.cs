using System;
using System.Collections.Generic;
using System.Linq;

namespace DiContainer.Core.MyAdvancedDiContainer
{
    public class MyDiContainer
    {
        private readonly Dictionary<Type, ServiceDescriptor> _serviceDescriptors;

        public MyDiContainer(Dictionary<Type, ServiceDescriptor> serviceDescriptors)
        {
            _serviceDescriptors = serviceDescriptors;
        }

        public TService GetService<TService>()
        {
            return (TService)GetService(typeof(TService));
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
}
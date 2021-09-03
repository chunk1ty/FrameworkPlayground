using System;
using System.Collections.Generic;
using System.Linq;

namespace DiContainer.Core.MyAdvancedDiContainer
{
    public class MyDiContainer : IServiceProvider
    {
        private readonly Dictionary<Type, MyServiceDescriptor> _serviceDescriptors;

        public MyDiContainer(Dictionary<Type, MyServiceDescriptor> serviceDescriptors)
        {
            _serviceDescriptors = serviceDescriptors;
        }

        public TService GetService<TService>()
        {
            return (TService)GetService(typeof(TService));
        }

        public object GetService(Type serviceType)
        {
            if (!_serviceDescriptors.ContainsKey(serviceType))
            {
                throw new InvalidOperationException($"No registration for: [{serviceType}].");
            }

            MyServiceDescriptor myServiceDescriptor = _serviceDescriptors[serviceType];

            if (myServiceDescriptor.ImplementationType.IsAbstract || myServiceDescriptor.ImplementationType.IsInterface)
            {
                throw new InvalidOperationException($"Cannot instantiate abstract class or interface: [{myServiceDescriptor.ImplementationType}].");
            }

            if (myServiceDescriptor.Implementation != null)
            {
                return myServiceDescriptor.Implementation;
            }

            if (myServiceDescriptor.ImplementationFactory != null)
            {
                return  myServiceDescriptor.ImplementationFactory.Invoke(this);
            }

            object implementationInstance = CreateNewInstance(myServiceDescriptor.ImplementationType);

            if (myServiceDescriptor.Lifetime == MyServiceLifetime.Singleton)
            {
                myServiceDescriptor.Implementation = implementationInstance;
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
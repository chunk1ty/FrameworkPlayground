using System;
using System.Collections.Generic;

namespace DiContainer.Core
{
    public class MySimpleContainer
    {
        static readonly Dictionary<Type, object> _registeredTypes = new Dictionary<Type, object>();

        public void Register<TInterface, TImplementation>(TImplementation toRegister)
        {
            _registeredTypes.Add(typeof(TInterface), toRegister);
        }

        public TInterface Resolve<TInterface>()
        {
            return (TInterface) _registeredTypes[typeof(TInterface)];
        }
    }
}
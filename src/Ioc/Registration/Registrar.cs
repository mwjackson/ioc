using System;
using System.Collections.Generic;

namespace Ioc.Registration
{
    public class Registrar
    {
        public Registrar()
        {
            Registrations = new Dictionary<Type, ObjectRegistration>();
        }

        public Dictionary<Type, ObjectRegistration> Registrations { get; private set; }
        private Type _typeKey;

        public Registrar Satisfy<T>()
        {
            Registrations.Add(typeof(T), null);
            _typeKey = typeof (T);
            return this;
        }

        public Registrar With<T>(T concrete)
        {
            if (!_typeKey.IsInstanceOfType(concrete))
                throw new ArgumentException(string.Format("Cannot satisy {0} with {1} - types are not compatible.", _typeKey, concrete.GetType()));
            
            Registrations[_typeKey] = new ConcreteRegistration(_typeKey, concrete);
            return this;
        }

        public Registrar With<T>()
        {
            Registrations[_typeKey] = new ConstructedRegistration<T>(_typeKey);
            return this;
        }

        public Registrar With<T>(dynamic explicitArguments)
        {
            
            return this;
        }

        public Registrar With(Func<object> factoryFunction)
        {
            //if (!_typeKey.IsAssignableFrom(typeof(factoryFunction.)))
            //    throw new ArgumentException(string.Format("Cannot satisy {0} with {1} - types are not compatible.", _typeKey, typeof(T)));

            Registrations[_typeKey] = new FactoryRegistration(_typeKey, factoryFunction);
            return this;
        }
    }
}
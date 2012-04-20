using System;
using System.Collections.Generic;

namespace Ioc.Registration
{
    public class Registrar
    {
        public Registrar()
        {
            Registrations = new List<ObjectRegistration>();
        }

        public List<ObjectRegistration> Registrations { get; private set; }
        private Type _typeKey;

        public Registrar Satisfy<T>()
        {
            _typeKey = typeof (T);
            return this;
        }

        public Registrar With<T>(T concrete)
        {
            if (!_typeKey.IsInstanceOfType(concrete))
                throw new ArgumentException(string.Format("Cannot satisy {0} with {1} - types are not compatible.", _typeKey, concrete.GetType()));
            
            Registrations.Add(new ConcreteRegistration(_typeKey, concrete));
            return this;
        }

        public Registrar With<T>()
        {
            Registrations.Add(new ConstructedRegistration<T>(_typeKey));
            return this;
        }

        public Registrar With<T>(dynamic explicitArguments)
        {
            
            return this;
        }

        public Registrar With<T>(Func<T> factoryFunction)
        {
            //if (!_typeKey.IsAssignableFrom(typeof(T)))
            //    throw new ArgumentException(string.Format("Cannot satisy {0} with {1} - types are not compatible.", _typeKey, typeof(T)));

            Registrations.Add(new FactoryRegistration<T>(_typeKey, factoryFunction));
            return this;
        }
    }
}
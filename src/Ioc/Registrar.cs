using System;
using System.Collections.Generic;

namespace Ioc
{
    public class Registrar
    {
        public Registrar()
        {
            Registrations = new Dictionary<Type, object>();
        }

        public Dictionary<Type, object> Registrations { get; private set; }
        private Type _typeKey;

        public Registrar Satisfy<T>()
        {
            Registrations.Add(typeof(T), null);
            _typeKey = typeof (T);
            return this;
        }

        public Registrar With(object concrete)
        {
            if (!_typeKey.IsInstanceOfType(concrete))
                throw new ArgumentException(string.Format("Cannot satisy {0} with {1} - types are not compatible.", _typeKey, concrete.GetType()));

            Registrations[_typeKey] = concrete;
            return this;
        }

        public Registrar With<T>()
        {
            Registrations[_typeKey] = typeof(T);
            return this;
        }

        public Registrar With<T>(Func<T> factoryFunction)
        {
            if (!_typeKey.IsAssignableFrom(typeof(T)))
                throw new ArgumentException(string.Format("Cannot satisy {0} with {1} - types are not compatible.", _typeKey, typeof(T)));

            Registrations[_typeKey] = factoryFunction;
            return this;
        }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            ValidateRegistration(_typeKey, typeof(T));

            Registrations.Add(new ConcreteRegistration(_typeKey, concrete));
            return this;
        }

        public Registrar With<T>()
        {
            ValidateRegistration(_typeKey, typeof(T));

            Registrations.Add(new ConstructedRegistration<T>(_typeKey));
            return this;
        }

        public Registrar With<T>(dynamic explicitArguments)
        {
            ValidateRegistration(_typeKey, typeof(T));

            var properties = (PropertyInfo[]) explicitArguments.GetType().GetProperties();

            var arguments = properties.ToDictionary(k => k.Name, v => v.GetValue(explicitArguments, null));

            Registrations.Add(new ConstructedRegistration<T>(_typeKey, arguments));
            return this;
        }

        public Registrar With<T>(Func<T> factoryFunction)
        {
            ValidateRegistration(_typeKey, typeof (T));

            Registrations.Add(new FactoryRegistration<T>(_typeKey, factoryFunction));
            return this;
        }

        private void ValidateRegistration(Type ofType, Type byType)
        {
            if (!ofType.IsAssignableFrom(byType))
                throw new ArgumentException(string.Format("Cannot satisy {0} with {1} - types are not compatible.", ofType, byType));
        }
    }
}
using System;
using System.Collections.Generic;
using Ioc.Registration;

namespace Ioc.Resolution
{
    public class Resolver
    {
        public Resolver(Dictionary<Type, ObjectRegistration> registrations)
        {
            Registrations = registrations;
        }

        private Dictionary<Type, ObjectRegistration> Registrations { get; set; }

        public T Resolve<T>()
        {
            if (!Registrations.ContainsKey(typeof(T)))
                throw new ArgumentException(string.Format("Type {0} has not been registered.", typeof(T)));

            var registration = Registrations[typeof(T)];

            if (registration is ConstructedRegistration<T>)
            {
                var type = (registration as ConstructedRegistration<T>).ByType;
                return (T) Activator.CreateInstance(type);
            }

            if (registration is FactoryRegistration)
            {
                var factoryRegistration = registration as FactoryRegistration;
                return (T) factoryRegistration.FactoryFunction();
            }

            return (T) (registration as ConcreteRegistration).Concrete;
        }
    }
}
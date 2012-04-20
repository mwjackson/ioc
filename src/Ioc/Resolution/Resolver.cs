using System;
using System.Collections.Generic;
using System.Linq;
using Ioc.Registration;

namespace Ioc.Resolution
{
    public class Resolver
    {
        public Resolver(List<ObjectRegistration> registrations)
        {
            Registrations = registrations;
        }

        private List<ObjectRegistration> Registrations { get; set; }

        public T Resolve<T>()
        {
            if (!Registrations.Any(r => r.ForType == typeof(T)))
                throw new ArgumentException(string.Format("Type {0} has not been registered.", typeof(T)));

            var registration = Registrations.First(r => r.ForType == typeof(T));

            if (registration is ConstructedRegistration<T>)
            {
                var type = (registration as ConstructedRegistration<T>).ByType;
                return (T) Activator.CreateInstance(type);
            }

            if (registration is ConcreteRegistration)
            {
                return (T)(registration as ConcreteRegistration).Concrete;
            }

            var factoryRegistration = registration as IFactoryRegistration<T>;
            return (T) factoryRegistration.FactoryFunction();
        }
    }
}
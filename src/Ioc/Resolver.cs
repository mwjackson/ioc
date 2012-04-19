using System;
using System.Collections.Generic;

namespace Ioc
{
    public class Resolver
    {
        public Resolver(Dictionary<Type, object> registrations)
        {
            Registrations = registrations;
        }

        private Dictionary<Type, object> Registrations { get; set; }

        public T Resolve<T>()
        {
            var registration = Registrations[typeof(T)];

            if (registration.GetType() == typeof(Type))
            {
                var type = registration as Type;
                return (T)Activator.CreateInstance(type);
            }

            if (registration.GetType() == typeof(Func<T>))
            {
                var factoryFunction = registration as Func<T>;
                return factoryFunction();
            }

            return (T)registration;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                return ConstructObject<T>(registration);
            }

            if (registration is ConcreteRegistration)
            {
                return (T)(registration as ConcreteRegistration).Concrete;
            }

            var factoryRegistration = registration as IFactoryRegistration<T>;
            return factoryRegistration.FactoryFunction();
        }

        private static T ConstructObject<T>(ObjectRegistration registration)
        {
            var constructedRegistration = (registration as ConstructedRegistration<T>);
            var greediestCtor = constructedRegistration.ByType.GetConstructors()
                .OrderByDescending(ctor => ctor.GetParameters().Length)
                .First();
            var requiredParameters = greediestCtor.GetParameters().ToDictionary(pi => pi.Name, pi => new object());
            MatchParams(requiredParameters, constructedRegistration.Parameters);
            return (T)greediestCtor.Invoke(requiredParameters.Select(x => x.Value).ToArray());
        }

        private static void MatchParams(Dictionary<string, object> requiredParameters, Dictionary<string, object> suppliedParameters)
        {
            foreach (var key in requiredParameters.Keys.ToList())
            {
                requiredParameters[key] = suppliedParameters[key];
            }
        }
    }
}
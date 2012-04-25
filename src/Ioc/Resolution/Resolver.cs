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

            if (registration is IConstructedRegistration<T>)
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
            var constructedRegistration = (registration as IConstructedRegistration<T>);
            var greediestCtor = constructedRegistration.ByType.GetConstructors()
                .OrderByDescending(ctor => ctor.GetParameters().Length)
                .First();
            var requiredParameters = greediestCtor.GetParameters().ToDictionary(pi => pi.Name.ToLower(), pi => new object());
            var suppliedParameters = constructedRegistration.Parameters;
            MatchParams(requiredParameters, suppliedParameters);
            return (T)greediestCtor.Invoke(requiredParameters.Select(x => x.Value).ToArray());
        }

        private static void MatchParams(Dictionary<string, object> requiredParameters, Dictionary<string, object> suppliedParameters)
        {
            if (requiredParameters.Count() > suppliedParameters.Count())
            {
                var missingArguments = requiredParameters.Except(suppliedParameters, new ArgumentComparer()).Select(x => x.Key).ToArray();
                throw new ArgumentException(string.Format("Missing parameters: {0}", string.Join(" ", missingArguments)));
            }

            foreach (var paramKey in suppliedParameters.Keys.ToList())
            {
                requiredParameters[paramKey.ToLower()] = suppliedParameters[paramKey];
            }
        }
    }

    internal class ArgumentComparer : IEqualityComparer<KeyValuePair<string, object>>
    {
        public bool Equals(KeyValuePair<string, object> argumentX, KeyValuePair<string, object> argumentY)
        {
            return argumentX.Key == argumentY.Key;
        }

        public int GetHashCode(KeyValuePair<string, object> argument)
        {
            unchecked
            {
                return ((argument.Key != null ? argument.Key.GetHashCode() : 0) * 397);
            }
        }
    }
}
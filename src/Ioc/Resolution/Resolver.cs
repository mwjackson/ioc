using System;
using System.Collections.Generic;
using System.Configuration;
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

            if (registration is ConcreteRegistration)
            {
                return (T)(registration as ConcreteRegistration).Concrete;
            }

            if (registration is IFactoryRegistration<T>)
            {
                var factoryRegistration = registration as IFactoryRegistration<T>;
                return factoryRegistration.FactoryFunction();
            }

            return ConstructObject<T>(registration);
        }

        private T ConstructObject<T>(ObjectRegistration registration)
        {
            var constructedRegistration = (registration as IConstructedRegistration<T>);
            var greediestCtor = constructedRegistration.ByType.GetConstructors()
                .OrderByDescending(ctor => ctor.GetParameters().Length)
                .First();
            var requiredParameters = greediestCtor.GetParameters().ToDictionary(pi => pi.Name.ToLower(), pi => new Parameter(pi));
            var suppliedParameters = constructedRegistration.Parameters;
            MatchSuppliedParams(requiredParameters, suppliedParameters);
            AttemptToResolveMissingParams(requiredParameters);
            AttemptToResolveValueTypesFromConfigurationFiles(requiredParameters);
            CheckForAllParameters(requiredParameters);
            return (T) greediestCtor.Invoke(requiredParameters.Select(x => x.Value.ParameterValue).ToArray());
        }

        private static void MatchSuppliedParams(Dictionary<string, Parameter> requiredParameters, Dictionary<string, object> suppliedParameters)
        {
            foreach (var paramKey in suppliedParameters.Keys.ToList())
            {
                requiredParameters[paramKey.ToLower()].ParameterValue = suppliedParameters[paramKey];
            }
        }

        private void AttemptToResolveValueTypesFromConfigurationFiles(Dictionary<string, Parameter> requiredParameters)
        {
            var stillToResolve = requiredParameters.Where(x => x.Value.ParameterValue is NullParameter).Select(x => x.Key).ToArray();
            foreach (var paramKey in stillToResolve)
            {
                var type = requiredParameters[paramKey].Type;
                if (!type.IsValueType && type != typeof(string)) continue;

                if (ConfigurationManager.AppSettings.AllKeys.Select(x => x.ToLower()).Contains(paramKey))
                {
                    requiredParameters[paramKey].ParameterValue = ConfigurationManager.AppSettings[paramKey];
                    continue;
                }

                var connectionStringSettingses = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>();
                if (connectionStringSettingses.Any(x => x.Name.ToLower() == paramKey))
                    requiredParameters[paramKey].ParameterValue = connectionStringSettingses.First(x => x.Name.ToLower() == paramKey).ConnectionString;
            }
        }

        private void AttemptToResolveMissingParams(Dictionary<string, Parameter> requiredParameters)
        {
            var stillToResolve = requiredParameters.Where(x => x.Value.ParameterValue is NullParameter).Select(x => x.Key).ToArray();
            foreach(var paramKey in stillToResolve)
            {
                var type = requiredParameters[paramKey].Type;
                if (type.IsValueType || type == typeof(string)) continue;
                requiredParameters[paramKey].ParameterValue = ResolveInternal(type);
            }
        }

        private object ResolveInternal(Type type)
        {
            MethodInfo method = GetType().GetMethod("Resolve");
            return method.MakeGenericMethod(type).Invoke(this, null);
        }

        private static void CheckForAllParameters(Dictionary<string, Parameter> requiredParameters)
        {
            if (requiredParameters.Any(x => x.Value.ParameterValue is NullParameter))
            {
                var missingArguments = requiredParameters.Where(x => x.Value.ParameterValue is NullParameter).Select(x => x.Key).ToArray();
                throw new ArgumentException(string.Format("Missing parameters: {0}", string.Join(" ", missingArguments)));
            }
        }

        private class Parameter
        {
            public Parameter(ParameterInfo parameterInfo)
            {
                ParameterValue = new NullParameter();
                Type = parameterInfo.ParameterType;
                Position = parameterInfo.Position;
            }

            public object ParameterValue { get; set; }
            public Type Type { get; set; }
            public int Position { get; set; }
        }

        private class NullParameter
        {
        }
    }
}
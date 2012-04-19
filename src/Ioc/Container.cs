using System;
using System.Collections.Generic;

namespace Ioc
{
    public class Container
    {
        public Container()
        {
            Registrations = new Dictionary<Type, object>();
        }

        public Dictionary<Type, object> Registrations;
        private Type _typeKey;

        public Container Satisfy<T>()
        {
            Registrations.Add(typeof(T), null);
            _typeKey = typeof (T);
            return this;
        }

        public Container With(object concrete)
        {
            Registrations[_typeKey] = concrete;
            return this;
        }

        public object Resolve<T>()
        {
            return Registrations[typeof (T)];
        }
    }
}
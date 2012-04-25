using System;
using System.Collections.Generic;

namespace Ioc.Registration
{
    public interface IConstructedRegistration<out T>
    {
        Type ByType { get; }
        Dictionary<string, object> Parameters { get;} 
    }

    public class ConstructedRegistration<T> : ObjectRegistration, IConstructedRegistration<T>
    {
        public ConstructedRegistration(Type forType) : base(forType)
        {
            ByType = typeof (T);
            Parameters = new Dictionary<string, object>();
        }

        public ConstructedRegistration(Type forType, Dictionary<string, object> arguments)
            : base(forType)
        {
            ByType = typeof(T);
            Parameters = arguments;
        }

        public Type ByType { get; private set; }
        public Dictionary<string, object> Parameters { get; private set; }
    }
}
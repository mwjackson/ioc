using System;
using System.Collections.Generic;

namespace Ioc.Registration
{
    public class ConstructedRegistration<T> : ObjectRegistration
    {
        public ConstructedRegistration(Type forType) : base(forType)
        {
            ByType = typeof (T);
            Arguments = new Dictionary<string, object>();
        }

        public ConstructedRegistration(Type forType, Dictionary<string, object> arguments)
            : base(forType)
        {
            ByType = typeof(T);
            Arguments = arguments;
        }

        public Type ByType { get; private set; }
        public Dictionary<string, object> Arguments { get; private set; } 
    }
}
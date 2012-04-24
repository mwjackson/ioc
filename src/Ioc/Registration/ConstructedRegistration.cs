using System;
using System.Collections.Generic;

namespace Ioc.Registration
{
    public class ConstructedRegistration<T> : ObjectRegistration
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
using System;

namespace Ioc.Registration
{
    public class FactoryRegistration : ObjectRegistration
    {
        public FactoryRegistration(Type forType, Func<object> factoryFunction) : base(forType)
        {
            FactoryFunction = factoryFunction;
        }

        public Func<object> FactoryFunction { get; private set; } 
    }
}
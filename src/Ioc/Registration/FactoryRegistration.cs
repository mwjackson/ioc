using System;

namespace Ioc.Registration
{
    public interface IFactoryRegistration<out T>
    {
        Func<T> FactoryFunction { get; }
    }

    public class FactoryRegistration<T> : ObjectRegistration, IFactoryRegistration<T>
    {
        public FactoryRegistration(Type forType, Func<T> factoryFunction) : base(forType)
        {
            FactoryFunction = factoryFunction;
        }

        public Func<T> FactoryFunction { get; private set; } 
    }
}
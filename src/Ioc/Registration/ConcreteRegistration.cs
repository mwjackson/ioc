using System;

namespace Ioc.Registration
{
    public class ConcreteRegistration : ObjectRegistration
    {
        public ConcreteRegistration(Type forType, object concrete) : base(forType)
        {
            Concrete = concrete;
        }

        public object Concrete { get; private set; } 
    }
}
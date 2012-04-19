using System;

namespace Ioc.Registration
{
    public abstract class ObjectRegistration
    {
        protected ObjectRegistration(Type forType)
        {
            ForType = forType;
        }

        public Type ForType { get; protected set; }
    }
}
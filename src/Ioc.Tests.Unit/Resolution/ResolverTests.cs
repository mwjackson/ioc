using System;
using System.Collections.Generic;
using Ioc.Registration;
using Ioc.Resolution;
using Ioc.Tests.Unit.Registration;
using NUnit.Framework;

namespace Ioc.Tests.Unit.Resolution
{
    [TestFixture]
    public class ResolverTests
    {
        [Test]
        public void Resolving_a_type_that_hasnt_been_registered_should_throw_invalidargument()
        {
            Assert.That(() => new Resolver(new Dictionary<Type, ObjectRegistration>()).Resolve<string>(), Throws.ArgumentException);
        }


        [Test]
        public void Resolving_an_existing_object_should_get_the_same_object_back_that_we_registered()
        {
            var registeredObject = new Test();

            var registrar = new Registrar().Satisfy<ITest>().With(registeredObject);

            var resolvedObject = new Resolver(registrar.Registrations).Resolve<ITest>();

            Assert.That(ReferenceEquals(registeredObject, resolvedObject), Is.True, "the 2 object references are not the same");
        }

        [Test]
        public void Resolving_a_factory_function_should_get_back_the_same_object_we_registered()
        {
            var registeredObject = new Test();
            Func<object> factoryFunction = () => registeredObject;

            var registrar = new Registrar().Satisfy<ITest>().With(factoryFunction);

            var resolvedObject = new Resolver(registrar.Registrations).Resolve<ITest>();

            Assert.That(ReferenceEquals(registeredObject, resolvedObject), Is.True, "the 2 object references are not the same");
        }

    }
}
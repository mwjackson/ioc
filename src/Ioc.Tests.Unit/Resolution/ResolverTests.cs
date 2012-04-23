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
            Assert.That(() => new Resolver(new List<ObjectRegistration>()).Resolve<string>(), Throws.ArgumentException);
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
            Func<Test> factoryFunction = () => registeredObject;

            var registrar = new Registrar().Satisfy<ITest>().With(factoryFunction);

            var resolvedObject = new Resolver(registrar.Registrations).Resolve<ITest>();

            Assert.That(ReferenceEquals(registeredObject, resolvedObject), Is.True, "the 2 object references are not the same");
        }

        [Test]
        public void Resolve_an_object_with_arguments_should_have_the_arguments_set()
        {
            string argument1 = "someArgument";
            decimal argument2 = 1234m;

            var registrar = new Registrar().Satisfy<ClassWithArguments>().With<ClassWithArguments>(new { Argument1 = argument1, Argument2 = argument2, Argument3 = ""});

            var resolvedObject = new Resolver(registrar.Registrations).Resolve<ClassWithArguments>();

            Assert.That(ReferenceEquals(resolvedObject.Argument1, argument1), Is.True, "expected argument 1 to be set");
            Assert.That(resolvedObject.Argument2, Is.EqualTo(argument2), "expected argument 2 to be set");
        }

        [Test]
        public void Resolve_an_object_with_arguments_of_same_type_but_different_names_should_set_them_in_the_right_order()
        {
            string argument1 = "someArgument";
            decimal argument2 = 1234m;
            string argument3 = "anotherArgument";

            var registrar = new Registrar().Satisfy<ClassWithArguments>().With<ClassWithArguments>(new { Argument3 = argument3, Argument1 = argument1, Argument2 = argument2 });

            var resolvedObject = new Resolver(registrar.Registrations).Resolve<ClassWithArguments>();

            Assert.That(ReferenceEquals(resolvedObject.Argument1, argument1), Is.True, "expected argument 1 to be set");
            Assert.That(resolvedObject.Argument2, Is.EqualTo(argument2), "expected argument 2 to be set");
            Assert.That(ReferenceEquals(resolvedObject.Argument3, argument3), Is.True, "expected argument 3 to be set");
        }
    }

    public class ClassWithArguments
    {
        public ClassWithArguments(string argument1, decimal argument2, string argument3)
        {
            Argument1 = argument1;
            Argument2 = argument2;
            Argument3 = argument3;
        }

        public string Argument1 { get; private set; }
        public decimal Argument2 { get; private set; }
        public string Argument3 { get; private set; }
    }
}
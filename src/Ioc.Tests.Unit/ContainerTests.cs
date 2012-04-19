﻿using System;
using NUnit.Framework;

namespace Ioc.Tests.Unit
{
    [TestFixture]
    public class ContainerTests
    {
        private Container _container;

        [SetUp]
        public void Setup()
        {
            _container = new Container();
        }

        [Test]
        public void Registering_an_existing_object_should_be_present_in_the_registrations_list()
        {
            var registeredObject = new Test();

            _container.Satisfy<ITest>().With(registeredObject);

            var registrations = _container.Registrations;

            Assert.That(registrations.ContainsValue(registeredObject), Is.True, "concrete has not been registered");   
        }

        [Test]
        public void Registering_an_existing_object_should_then_be_present_in_the_registrations_list_for_an_interface()
        {
            _container.Satisfy<ITest>().With(new Test());

            var registrations = _container.Registrations;

            Assert.That(registrations.ContainsKey(typeof(ITest)), Is.True, "interface has not been registered");   
        }

        [Test]
        public void Registering_a_concrete_that_is_not_a_type_of_parent_should_throw_invalidargument()
        {
            Assert.That(() => _container.Satisfy<ITest>().With("someStringThatIsNotTypeOfIInterface"), Throws.ArgumentException, "should not be able to register incompatible types");
        }

        [Test]
        public void Registering_a_factory_function_should_be_present_in_the_registrations_list()
        {
            Func<ITest> factoryFunction = () => new Test();

            _container.Satisfy<ITest>().With(factoryFunction);

            var registrations = _container.Registrations;
            Assert.That(registrations.ContainsKey(typeof(ITest)), Is.True, "interface has not been registered");
            Assert.That(registrations[typeof(ITest)], Is.TypeOf<Func<ITest>>(), "should have registered a factory function for that interface!");
        }

        [Test]
        public void Registering_a_factoryfunction_that_is_not_a_type_of_parent_should_throw_invalidargument()
        {
            Func<string> factoryFunction = () => "someString";

            Assert.That(() => _container.Satisfy<ITest>().With(factoryFunction), Throws.ArgumentException, "should not be able to register incompatible types");
        }

        [Test]
        public void Resolving_an_existing_object_should_get_the_same_object_back_that_we_registered()
        {
            var registeredObject = new Test();

            _container.Satisfy<ITest>().With(registeredObject);

            var resolvedObject = _container.Resolve<ITest>();

            Assert.That(ReferenceEquals(registeredObject, resolvedObject), Is.True, "the 2 object references are not the same");
        }

        [Test]
        public void Resolving_a_factory_function_should_get_back_the_same_object_we_registered()
        {
            var registeredObject = new Test();
            Func<ITest> factoryFunction = () => registeredObject;

            _container.Satisfy<ITest>().With(factoryFunction);

            var resolvedObject = _container.Resolve<ITest>();

            Assert.That(ReferenceEquals(registeredObject, resolvedObject), Is.True, "the 2 object references are not the same");
        }

        [Test]
        public void Registering_a_type_should_be_present_in_the_registrations_list()
        {
            _container.Satisfy<ITest>().With<Test>();
            
            var registrations = _container.Registrations;
            Assert.That(registrations.ContainsKey(typeof(ITest)), Is.True, "interface has not been registered");
            Assert.That(registrations[typeof(ITest)], Is.EqualTo(typeof(Test)), "should have registered the type for that interface!");
        }

        [Test]
        public void Registering_a_type_with_ctor_args_should_take_argument_list ()
        {
            Assert.Fail("pending");
        }
    }

    public interface ITest { }

    public class Test : ITest {}

    public class TestWithCtorArgs : ITest
    {
        public TestWithCtorArgs(string arg1, decimal arg2)
        {
            
        }
    }
}
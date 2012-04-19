using System;
using NUnit.Framework;

namespace Ioc.Tests.Unit
{
    [TestFixture]
    public class RegistrarTests
    {
        private Registrar _registrar;

        [SetUp]
        public void Setup()
        {
            _registrar = new Registrar();
        }

        [Test]
        public void Registering_an_existing_object_should_be_present_in_the_registrations_list()
        {
            var registeredObject = new Test();

            _registrar.Satisfy<ITest>().With(registeredObject);

            var registrations = _registrar.Registrations;

            Assert.That(registrations.ContainsValue(registeredObject), Is.True, "concrete has not been registered");   
        }

        [Test]
        public void Registering_an_existing_object_should_then_be_present_in_the_registrations_list_for_an_interface()
        {
            _registrar.Satisfy<ITest>().With(new Test());

            var registrations = _registrar.Registrations;

            Assert.That(registrations.ContainsKey(typeof(ITest)), Is.True, "interface has not been registered");   
        }

        [Test]
        public void Registering_a_concrete_that_is_not_a_type_of_parent_should_throw_invalidargument()
        {
            Assert.That(() => _registrar.Satisfy<ITest>().With("someStringThatIsNotTypeOfIInterface"), Throws.ArgumentException, "should not be able to register incompatible types");
        }

        [Test]
        public void Registering_a_factory_function_should_be_present_in_the_registrations_list()
        {
            Func<ITest> factoryFunction = () => new Test();

            _registrar.Satisfy<ITest>().With(factoryFunction);

            var registrations = _registrar.Registrations;
            Assert.That(registrations.ContainsKey(typeof(ITest)), Is.True, "interface has not been registered");
            Assert.That(registrations[typeof(ITest)], Is.TypeOf<Func<ITest>>(), "should have registered a factory function for that interface!");
        }

        [Test]
        public void Registering_a_factoryfunction_that_is_not_a_type_of_parent_should_throw_invalidargument()
        {
            Func<string> factoryFunction = () => "someString";

            Assert.That(() => _registrar.Satisfy<ITest>().With(factoryFunction), Throws.ArgumentException, "should not be able to register incompatible types");
        }


        [Test]
        public void Registering_a_type_should_be_present_in_the_registrations_list()
        {
            _registrar.Satisfy<ITest>().With<Test>();
            
            var registrations = _registrar.Registrations;
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
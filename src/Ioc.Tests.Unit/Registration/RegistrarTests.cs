using System;
using System.Linq;
using Ioc.Registration;
using NUnit.Framework;

namespace Ioc.Tests.Unit.Registration
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
        public void Registering_a_concrete_instance_should_then_be_present_in_the_registrations_list_for_an_interface()
        {
            var concreteInstance = new Test();
            _registrar.Satisfy<ITest>().With(concreteInstance);

            var registration = _registrar.Registrations.First();
            Assert.That(registration, Is.InstanceOf<ConcreteRegistration>(), "concrete has not been registered");
            Assert.That(registration.ForType, Is.EqualTo(typeof (ITest)));
            Assert.That(ReferenceEquals((registration as ConcreteRegistration).Concrete, concreteInstance), Is.True, "didnt get back instance we registered");
        }

        [Test]
        public void Registering_a_concrete_that_is_not_a_type_of_parent_should_throw_invalidargument()
        {
            Assert.That(() => _registrar.Satisfy<ITest>().With("someStringThatIsNotTypeOfIInterface"), Throws.ArgumentException, "should not be able to register incompatible types");
        }

        [Test]
        public void Registering_a_factory_function_should_be_present_in_the_registrations_list()
        {
            Func<Test> factoryFunction = () => new Test();

            _registrar.Satisfy<ITest>().With(factoryFunction);

            var registration = _registrar.Registrations.First();
            Assert.That(registration, Is.TypeOf<FactoryRegistration<Test>>(), "should have registered a factory function for that interface!");
            Assert.That(registration.ForType, Is.EqualTo(typeof(ITest)));
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
            
            var registration = _registrar.Registrations.First();

            Assert.That(registration, Is.InstanceOf<ConstructedRegistration<Test>>(), "should have registered the type for that interface!");
            Assert.That(registration.ForType, Is.EqualTo(typeof(ITest)));
            Assert.That((registration as ConstructedRegistration<Test>).ByType, Is.EqualTo(typeof(Test)));
        }

        [Test]
        public void Registering_a_type_with_ctor_args_should_take_argument_list ()
        {
            Assert.Fail("pending");

            var arguments = new {Arg1 = "arg1", Arg2 = "arg"};
            _registrar.Satisfy<ITest>().With<Test>(arguments);

            //var registration = _registrar.Registrations.First();
            //Assert.That(registration.Key, Is.EqualTo(typeof(ITest)), "interface has not been registered");
            //Assert.That((registration.Value as ConstructedRegistration<ITest>).Arguments, Is.EqualTo(arguments));
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
using System.Linq;
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

            Assert.That(registrations.Any(x => x.Value == registeredObject), Is.True, "concrete has not been registered");   
        }

        [Test]
        public void Registering_an_existing_object_should_then_be_present_in_the_registrations_list_for_an_interface()
        {
            _container.Satisfy<ITest>().With(new Test());

            var registrations = _container.Registrations;

            Assert.That(registrations.Any(x => x.Key == typeof(ITest)), Is.True, "interface has not been registered");   
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
        public void Registering_a_concrete_that_is_not_a_type_of_parent_should_throw_invalidargument()
        {
            Assert.That(() => _container.Satisfy<ITest>().With("someStringThatIsNotTypeOfIInterface"), Throws.ArgumentException, "should not be able to register incompatible types");
        }
    }

    public interface ITest { }

    public class Test : ITest {}
}
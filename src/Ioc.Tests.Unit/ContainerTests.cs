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
            var registeredObject = new object();

            _container.Satisfy<IInterface>().With(registeredObject);

            var registrations = _container.Registrations;

            Assert.That(registrations.Any(x => x.Value == registeredObject), Is.True, "concrete has not been registered");   
        }

        [Test]
        public void Registering_an_existing_object_should_then_be_present_in_the_registrations_list_for_an_interface()
        {
            _container.Satisfy<IInterface>().With(new object());

            var registrations = _container.Registrations;

            Assert.That(registrations.Any(x => x.Key == typeof(IInterface)), Is.True, "interface has not been registered");   
        }

        [Test]
        public void Resolving_an_existing_object_should_get_the_same_object_back_that_we_registered()
        {
            var registeredObject = new object();

            _container.Satisfy<IInterface>().With(registeredObject);

            var resolvedObject = _container.Resolve<IInterface>();

            Assert.That(ReferenceEquals(registeredObject, resolvedObject), Is.True, "the 2 object references are not the same!");
        }
    }

    public interface IInterface
    {
    }
}
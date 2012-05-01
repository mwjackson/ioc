using Ioc.Registration;
using Ioc.Scanning;
using NUnit.Framework;
using System.Linq;

namespace Ioc.Tests.Unit.Scanning
{
    [TestFixture]
    public class ScannerTests
    {
        private Registrar _registrar;
        private Scanner _scanner;

        [SetUp]
        public void Setup()
        {
            _registrar = new Registrar();
            _scanner = new Scanner(_registrar);
        }

        [Test]
        public void Scanning_should_scan_an_assembly()
        {
            var thisType = GetType();
            _scanner.Scan(thisType.Assembly);

            Assert.That(_registrar.Registrations.Any(), Is.True);
        }
    }
}
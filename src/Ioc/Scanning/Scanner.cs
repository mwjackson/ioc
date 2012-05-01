using System.Linq;
using System.Reflection;
using Ioc.Registration;

namespace Ioc.Scanning
{
    public class Scanner
    {
        public Registrar Registrar { get; private set; }

        public Scanner(Registrar registrar)
        {
            Registrar = registrar;
        }

        public void Scan(Assembly assembly)
        {
            MethodInfo with = Registrar.GetType().GetMethods().First(x => x.Name == "With" && x.IsGenericMethod && !x.GetParameters().Any());

            var type = assembly.GetTypes().First();
            Registrar.Satisfy(type);
            with.MakeGenericMethod(type).Invoke(Registrar, null);
        }
    }
}
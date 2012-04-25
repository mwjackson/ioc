using System;
using System.Collections.Generic;
using Ioc.Registration;
using Ioc.Resolution;
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
            var registeredObject = new ClassWithNoArguments();

            var registrar = new Registrar().Satisfy<IClass>().With(registeredObject);

            var resolvedObject = new Resolver(registrar.Registrations).Resolve<IClass>();

            Assert.That(ReferenceEquals(registeredObject, resolvedObject), Is.True, "the 2 object references are not the same");
        }

        [Test]
        public void Resolving_a_factory_function_should_get_back_the_same_object_we_registered()
        {
            var registeredObject = new ClassWithNoArguments();
            Func<IClass> factoryFunction = () => registeredObject;

            var registrar = new Registrar().Satisfy<IClass>().With(factoryFunction);

            var resolvedObject = new Resolver(registrar.Registrations).Resolve<IClass>();

            Assert.That(ReferenceEquals(registeredObject, resolvedObject), Is.True, "the 2 object references are not the same");
        }

        [Test]
        public void Resolving_an_object_that_needs_be_be_constructed_should_return_an_instance_of_that_type()
        {
            var registrar = new Registrar().Satisfy<IClass>().With<ClassWithNoArguments>();

            var resolvedObject = new Resolver(registrar.Registrations).Resolve<IClass>();

            Assert.That(resolvedObject, Is.InstanceOf<ClassWithNoArguments>(), "expected resolved object to be of type ClassWithNoArguments");
        }

        [Test]
        public void Resolve_an_object_with_arguments_should_have_the_arguments_set()
        {
            string argument1 = "someArgument";
            decimal argument2 = 1234m;

            var registrar = new Registrar().Satisfy<ClassWithThreeArguments>().With<ClassWithThreeArguments>(new { Argument1 = argument1, Argument2 = argument2, Argument3 = ""});

            var resolvedObject = new Resolver(registrar.Registrations).Resolve<ClassWithThreeArguments>();

            Assert.That(ReferenceEquals(resolvedObject.Argument1, argument1), Is.True, "expected argument 1 to be set");
            Assert.That(resolvedObject.Argument2, Is.EqualTo(argument2), "expected argument 2 to be set");
        }

        [Test]
        public void Resolve_an_object_with_arguments_of_same_type_but_different_names_should_set_them_in_the_right_order()
        {
            string arg1 = "someArgument";
            decimal arg2 = 1234m;
            string arg3 = "anotherArgument";

            var registrar = new Registrar().Satisfy<ClassWithThreeArguments>().With<ClassWithThreeArguments>(new { argument3 = arg3, argument1 = arg1, argument2 = arg2 });

            var resolvedObject = new Resolver(registrar.Registrations).Resolve<ClassWithThreeArguments>();

            Assert.That(ReferenceEquals(resolvedObject.Argument1, arg1), Is.True, "expected argument 1 to be set");
            Assert.That(resolvedObject.Argument2, Is.EqualTo(arg2), "expected argument 2 to be set");
            Assert.That(ReferenceEquals(resolvedObject.Argument3, arg3), Is.True, "expected argument 3 to be set");
        }

        [Test]
        public void Resolving_an_object_with_names_in_different_case_to_ctor_should_still_set_them_correctly()
        {
            string arg1 = "someArgument";

            var registrar = new Registrar().Satisfy<ClassWithOneArgument>().With<ClassWithOneArgument>(new { ARGUMENT1 = arg1 });

            var resolvedObject = new Resolver(registrar.Registrations).Resolve<ClassWithOneArgument>();

            Assert.That(ReferenceEquals(resolvedObject.Argument1, arg1), Is.True, "expected argument 1 to be set");
        }

        [Test]
        public void Resolving_an_object_with_arguments_missing_should_throw_argumentexception()
        {
            string arg3 = "anotherArgument";

            var registrar = new Registrar().Satisfy<ClassWithThreeArguments>().With<ClassWithThreeArguments>(new { argument3 = arg3});

            Assert.That(() => new Resolver(registrar.Registrations).Resolve<ClassWithThreeArguments>(),
                Throws.ArgumentException.With.Message.EqualTo("Missing parameters: argument1 argument2"));
        }

        [Test]
        public void Resolving_an_object_with_argument_of_type_that_has_been_registered_should_resolve_that_type_also()
        {
            var dependency = new ClassWithNoArguments();
            var registrar = new Registrar().Satisfy<ClassWithSingleDependency>().With<ClassWithSingleDependency>()
                .Satisfy<ClassWithNoArguments>().With(dependency);

            var resolvedObject = new Resolver(registrar.Registrations).Resolve<ClassWithSingleDependency>();

            Assert.That(resolvedObject, Is.InstanceOf<ClassWithSingleDependency>(), "expected resolved object to be of type ClassWithSingleDependency");
            Assert.That(resolvedObject.Argument1, Is.EqualTo(dependency), "expected dependency to have been resolved also");
        }
    }

    public interface IClass
    {
        
    }

    public class ClassWithNoArguments : IClass
    {
        
    }

    public class ClassWithOneArgument : IClass
    {
        public ClassWithOneArgument(string argument1)
        {
            Argument1 = argument1;
        }

        public string Argument1 { get; private set; }
    }

    public class ClassWithThreeArguments : IClass
    {
        public ClassWithThreeArguments(string argument1, decimal argument2, string argument3)
        {
            Argument1 = argument1;
            Argument2 = argument2;
            Argument3 = argument3;
        }

        public string Argument1 { get; private set; }
        public decimal Argument2 { get; private set; }
        public string Argument3 { get; private set; }
    }

    public class ClassWithSingleDependency : IClass
    {
        public ClassWithNoArguments Argument1 { get; set; }

        public ClassWithSingleDependency(ClassWithNoArguments argument1)
        {
            Argument1 = argument1;
        }
    }
}
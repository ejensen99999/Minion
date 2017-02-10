using System;
using Minion.Tests.AssortedTestClasses;
using Xunit;
using Minion.Ioc.Profiler;

namespace Minion.Tests.IocTests.Unit
{
    public class ConstructorProfileTests
    {

        [Fact]
        public void get_constructors_from_interface_mapping1()
        {
            var ctors = ConstructorProfiler.GetConstructors(typeof(ClockEvent));

            Assert.Equal(1, ctors.Length);
        }

        [Fact]
        public void get_constructors_from_interface_mapping2()
        {
            var ctors = ConstructorProfiler.GetConstructors(typeof(TestClass1));

            Assert.Equal(7, ctors.Length);
        }

        [Fact]
        public void get_constructors_from_implementation_mapping1()
        {
            var ctors = ConstructorProfiler.GetConstructors(typeof(ClockEvent));

            Assert.Equal(1, ctors.Length);
        }

        [Fact]
        public void get_constructors_from_implementation_mapping2()
        {
            var ctors = ConstructorProfiler.GetConstructors(typeof(TestClass1));

            Assert.Equal(7, ctors.Length);
        }

        [Fact]
        public void get_constructors_from_interface_with_no_concrete()
        {
            var ctors = ConstructorProfiler.GetConstructors(typeof(ITestClass));

            Assert.Equal(0, ctors.Length);
        }

        [Fact]
        public void get_constructors_from_interface_with_no_contract()
        {
                var ctors = ConstructorProfiler.GetConstructors(typeof(TestClass1));

                Assert.Equal(7, ctors.Length);
        }

        [Fact]
        public void get_constructor_from_constructor_list()
        {
            var ctors = ConstructorProfiler.GetConstructors(typeof(TestClass1));
            var parms = ConstructorProfiler.GetConstructorProfile(ctors);

            Assert.Equal(7, ctors.Length);
            Assert.Equal(7, parms.Count);
        }

        [Fact]
        public void get_constructor_from_preferred_attribute()
        {
            var ctors = ConstructorProfiler.GetConstructors(typeof(TestClass2));
            var parms = ConstructorProfiler.GetConstructorProfile(ctors);

            Assert.Equal(7, ctors.Length);
            Assert.Equal(1, parms.Count);
        }

        [Fact]
        public void get_constructor_catch_exception()
        {
            Assert.Throws<NullReferenceException>(() =>
            {
                var parms = ConstructorProfiler.GetConstructorProfile(null);
            });

        }

        [Fact]
        public void get_optimal_constructor_from_constructor_list()
        {
            var ctors = ConstructorProfiler.GetConstructors(typeof(TestClass1));
            var parms = ConstructorProfiler.GetConstructorProfile(ctors);
            var ctor = ConstructorProfiler.GetOptimalConstructor(parms);

            Assert.Equal(7, ctors.Length);
            Assert.Equal(4, ctor.Magnitude);
            Assert.True(ctor.IsValid);
        }

        [Fact]
        public void get_optimal_constructor_from_preferred_attribute()
        {
            var ctors = ConstructorProfiler.GetConstructors(typeof(TestClass2));
            var parms = ConstructorProfiler.GetConstructorProfile(ctors);
            var ctor = ConstructorProfiler.GetOptimalConstructor(parms);

            Assert.Equal(7, ctors.Length);
            Assert.Equal(3, ctor.Magnitude);
            Assert.True(ctor.IsValid);
        }
    }
}

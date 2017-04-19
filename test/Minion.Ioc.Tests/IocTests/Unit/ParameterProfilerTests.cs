using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Minion.Ioc.Interfaces;
using Minion.Ioc.Profiler;
using Minion.Tests.AssortedTestClasses;
using Xunit;

namespace Minion.Tests.IocTests.Unit
{
    public class ParameterProfilerTests
    {
        private IParameterProfile setup(int whichClass = 0)
        {
            Type target = null;

            switch (whichClass)
            {
                case 0:
                    target = typeof (ParameterProfileTestClass0);
                    break;
                case 1:
                    target = typeof (ParameterProfileTestClass1);
                    break;
                case 2:
                    target = typeof (ParameterProfileTestClass2);
                    break;
                case 3:
                    target = typeof (ParameterProfileTestClass3);
                    break;
                case 4:
                    target = typeof (ParameterProfileTestClass4);
                    break;
                default:

                    break;
            }

            var ctors = target
                .GetTypeInfo()
                .GetConstructors();

            return new ParameterProfiler(ctors[0]);
        }

        private IParameterProfile get_base_tests(int paramCount)
        {
            var index = paramCount;
            var target = setup(index);
            var expected = index;

            var actualParameters = target.Parameters;
            var actualCount = actualParameters.Count;

            Assert.Equal(expected, actualCount);
            Assert.Equal(expected, target.Magnitude);
            Assert.True(target.IsValid);

            return target;
        }

        [Fact]
        public void zero_parameter()
        {
            var target = get_base_tests(0);

            var actualParameters = target.Parameters;
        }

        [Fact]
        public void one_parameter()
        {
            var target = get_base_tests(1);

            var actualParameters = target.Parameters;
            var actualCount = actualParameters.Count;

            Assert.Equal(typeof(IClockEvent), actualParameters[0].ContractType);
        }

        [Fact]
        public void two_parameters()
        {
            var target = get_base_tests(2);

            var actualParameters = target.Parameters;
            var actualCount = actualParameters.Count;

            Assert.Equal(typeof(IClockEvent), actualParameters[0].ContractType);
            Assert.Equal(typeof(IOrder), actualParameters[1].ContractType);
        }

        [Fact]
        public void three_parameters()
        {
            var target = get_base_tests(3);

            var actualParameters = target.Parameters;
            var actualCount = actualParameters.Count;

            Assert.Equal(typeof(IClockEvent), actualParameters[0].ContractType);
            Assert.Equal(typeof(IOrder), actualParameters[1].ContractType);
            Assert.Equal(typeof(ICubeTestParams), actualParameters[2].ContractType);
        }

        [Fact]
        public void four_parameters()
        {
            var target = get_base_tests(4);

            var actualParameters = target.Parameters;
            var actualCount = actualParameters.Count;

            Assert.Equal(typeof(IClockEvent), actualParameters[0].ContractType);
            Assert.Equal(typeof(IOrder), actualParameters[1].ContractType);
            Assert.Equal(typeof(ICubeTestParams), actualParameters[2].ContractType);
            Assert.Equal(typeof(List<string>), actualParameters[3].ContractType);
        }

        [Fact]
        public void test_had_default_constructor()
        {
            var target = typeof(List<string>).GetTypeInfo();

            var actual = ParameterProfiler.HasDefaultConstructor(target);

            Assert.True(actual);
        }

        [Fact]
        public void test_does_not_have_default_constructor()
        {
            var target = typeof(NoDefaultConstructor).GetTypeInfo();

            var actual = ParameterProfiler.HasDefaultConstructor(target);

            Assert.False(actual);
        }

        [Fact]
        public void test_class_is_acceptable_parameter()
        {
            var target = typeof(ClockEvent);

            TypeInfo info;
            var actual = ParameterProfiler.IsAcceptableParameter(target, out info);

            Assert.True(actual);
        }

        [Fact]
        public void test_interface_is_acceptable_parameter()
        {
            var target = typeof(IClockEvent);

            TypeInfo info;
            var actual = ParameterProfiler.IsAcceptableParameter(target, out info);

            Assert.True(actual);
        }

        [Fact]
        public void test_enum_is_not_acceptable_parameter()
        {
            var target = typeof(ServiceLifetime);

            TypeInfo info;
            var actual = ParameterProfiler.IsAcceptableParameter(target, out info);

            Assert.False(actual);
        }

        [Fact]
        public void test_value_is_not_acceptable_parameter()
        {
            var target = typeof(int);

            TypeInfo info;
            var actual = ParameterProfiler.IsAcceptableParameter(target, out info);

            Assert.False(actual);
        }
    }
}

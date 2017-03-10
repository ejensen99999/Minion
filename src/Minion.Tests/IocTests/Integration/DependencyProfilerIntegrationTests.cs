using System;
using Minion.Tests.AssortedTestClasses;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Minion.Ioc;
using Minion.Ioc.Exceptions;
using Minion.Ioc.Profiler;
using Minion.Ioc.Builders;
using System.Linq;
using Minion.Ioc.Interfaces;
using Minion.Ioc.Aspects;

namespace Minion.Tests.IocTests.Integration
{
    public class BuilderProfilerIntegrationTests
    {
        [Fact]
        public void test_set_mapping_with_registration_throws_exception()
        {
            var log = new Mock<ILogger>();
            var profiler = new DependencyProfiler(log.Object, new ConstructorProfiler(new TypeCache(new PassThroughEmitter())));
            var contract = typeof(IClockEvent);
            var concrete = typeof(ClockEvent);

            profiler.SetMapping(
                contract,
                concrete,
                Lifetime.Singleton,
                null);

            var ex = Assert.Throws<IocRegistrationException>(() =>
            {
                profiler.SetMapping(
                contract,
                concrete,
                Lifetime.Singleton,
                null);
            });
        }

        [Fact]
        public void test_set_mapping_throws_no_valid_constructor_exception()
        {
            var log = new Mock<ILogger>();
            var profiler = new DependencyProfiler(log.Object, new ConstructorProfiler(new TypeCache(new PassThroughEmitter())));
            var contract = typeof(NoValidConstructor);
            var concrete = typeof(NoValidConstructor);

            var ex = Assert.Throws<InvalidConstructorException>(() =>
            {
                profiler.SetMapping(
                contract,
                concrete,
                Lifetime.Singleton,
                null);
            });
        }
    }
}

using System;
using Minion.Tests.AssortedTestClasses;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Minion.Ioc;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Minion.Ioc.Profiler;
using Minion.Ioc.Builders;
using Minion.Ioc.Aspects;

namespace Minion.Tests.IocTests.Integration
{
    public class ContainerIntegrationTests
    {
        private Container getContainer()
        {
            var log = new Mock<ILogger>().Object;
          
            var profiler = new DependencyProfiler(log, new ConstructorProfiler(new TypeCache(new AspectEmitter())));
            var resolver = new DepedencyResolver(log, profiler);
            var container = new Container(log, profiler, resolver);

            return container;
        }

        private ClockEvent getTestObj()
        {
            var clid = 123456;
            var cid = 9696;
            var cname = "Test Company";
            var eid = Guid.NewGuid();
            var ename = "John Doe";
            var cevent = DateTime.Now;
            var sid = Guid.NewGuid(); ;
            var sname = "Matt Bastage";
            var tc = DateTime.Now;
            var tcevent = ClockEventTypes.LunchEnd;

            var obj = new ClockEvent
            {
                ClockId = clid,
                CompanyId = cid,
                CompanyName = cname,
                EmployeeId = eid,
                EmployeeName = ename,
                Event = cevent,
                SupervisorId = sid,
                SupervisorName = sname,
                TimeCard = tc,
                Type = tcevent,
            };

            return obj;
        }

        [Fact]
        public void container_add_registration_transient1()
        {
            var container = getContainer();

            container.Add<ClockEvent>(Lifetime.Transient);

            var actual = container.Get<ClockEvent>();

            Assert.IsType<ClockEvent>(actual);
        }

        [Fact]
        public void container_add_registration_transient2()
        {
            var container = getContainer();

            container.Add<IClockEvent, ClockEvent>(Lifetime.Transient);

            var actual = container.Get<IClockEvent>();

            Assert.IsType<ClockEvent>(actual);
        }

        [Fact]
        public void container_add_registration_transient_check_different()
        {
            var container = getContainer();

            container.Add<ClockEvent>(Lifetime.Transient);

            var actual1 = container.Get<ClockEvent>();
            var actual2 = container.Get<ClockEvent>();
            actual2.ClockId = 987654321;

            Assert.NotEqual(actual1.ClockId, actual2.ClockId);

        }

        [Fact]
        public void container_add_transient_registration1()
        {
            var container = getContainer();

            container.AddTransient<ClockEvent>();

            var actual = container.Get<ClockEvent>();

            Assert.IsType<ClockEvent>(actual);
        }

        [Fact]
        public void container_add_transient_registration2()
        {
            var container = getContainer();

            container.AddTransient<IClockEvent, ClockEvent>();

            var actual = container.Get<IClockEvent>();

            Assert.IsType<ClockEvent>(actual);
        }

        [Fact]
        public void container_add_transient_registration_check_different()
        {
            var container = getContainer();

            container.AddTransient<IClockEvent, ClockEvent>();

            var actual1 = container.Get<IClockEvent>();
            var actual2 = container.Get<IClockEvent>();
            actual2.ClockId = 987654321;

            Assert.NotEqual(actual1, actual2);
        }

        [Fact]
        public void container_add_transient_registration_with_initiator()
        {
            var container = getContainer();
            var clid = 123456;
            var cid = 9696;
            var cname = "Test Company";
            var eid = Guid.NewGuid();
            var ename = "John Doe";
            var cevent = DateTime.Now;
            var sid = Guid.NewGuid(); ;
            var sname = "Matt Bastage";
            var tc = DateTime.Now;
            var tcevent = ClockEventTypes.LunchEnd;

            container.AddTransient<IClockEvent, ClockEvent>(x => new ClockEvent
            {
                ClockId = clid,
                CompanyId = cid,
                CompanyName = cname,
                EmployeeId = eid,
                EmployeeName = ename,
                Event = cevent,
                SupervisorId = sid,
                SupervisorName = sname,
                TimeCard = tc,
                Type = tcevent,
            });

            var actual = container.Get<IClockEvent>();

            Assert.Equal(clid, actual.ClockId);
            Assert.Equal(cid, actual.CompanyId);
            Assert.Equal(cname, actual.CompanyName);
            Assert.Equal(eid, actual.EmployeeId);
            Assert.Equal(ename, actual.EmployeeName);
            Assert.Equal(cevent, actual.Event);
            Assert.Equal(sid, actual.SupervisorId);
            Assert.Equal(sname, actual.SupervisorName);
            Assert.Equal(tc, actual.TimeCard);
            Assert.Equal(tcevent, actual.Type);
        }

        [Fact]
        public void container_add_registration_singleton()
        {
            var container = getContainer();

            container.AddSingleton<ClockEvent>();

            var actual = container.Get<ClockEvent>();

            Assert.IsType<ClockEvent>(actual);

        }

        [Fact]
        public void container_add_registration_singleton_check_same()
        {
            var container = getContainer();

            container.AddSingleton<ClockEvent>();

            var actual1 = container.Get<ClockEvent>();
            var actual2 = container.Get<ClockEvent>();
            actual2.ClockId = 987654321;

            Assert.Equal(actual1, actual2);

        }

        [Fact]
        public void container_add_singleton_registration_with_initiator1()
        {
            var container = getContainer();
            var clid = 123456;
            var cid = 9696;
            var cname = "Test Company";
            var eid = Guid.NewGuid();
            var ename = "John Doe";
            var cevent = DateTime.Now;
            var sid = Guid.NewGuid(); ;
            var sname = "Matt Bastage";
            var tc = DateTime.Now;
            var tcevent = ClockEventTypes.LunchEnd;

            container.AddSingleton<IClockEvent, ClockEvent>(x => new ClockEvent
            {
                ClockId = clid,
                CompanyId = cid,
                CompanyName = cname,
                EmployeeId = eid,
                EmployeeName = ename,
                Event = cevent,
                SupervisorId = sid,
                SupervisorName = sname,
                TimeCard = tc,
                Type = tcevent,
            });

            var actual1 = container.Get<IClockEvent>();
            var actual2 = container.Get<IClockEvent>();

            Assert.Equal(clid, actual1.ClockId);
            Assert.Equal(cid, actual1.CompanyId);
            Assert.Equal(cname, actual1.CompanyName);
            Assert.Equal(eid, actual1.EmployeeId);
            Assert.Equal(ename, actual1.EmployeeName);
            Assert.Equal(cevent, actual1.Event);
            Assert.Equal(sid, actual1.SupervisorId);
            Assert.Equal(sname, actual1.SupervisorName);
            Assert.Equal(tc, actual1.TimeCard);
            Assert.Equal(tcevent, actual1.Type);

            Assert.Equal(actual1.ClockId, actual2.ClockId);
            Assert.Equal(actual1.CompanyId, actual2.CompanyId);
            Assert.Equal(actual1.CompanyName, actual2.CompanyName);
            Assert.Equal(actual1.EmployeeId, actual2.EmployeeId);
            Assert.Equal(actual1.EmployeeName, actual2.EmployeeName);
            Assert.Equal(actual1.Event, actual2.Event);
            Assert.Equal(actual1.SupervisorId, actual2.SupervisorId);
            Assert.Equal(actual1.SupervisorName, actual2.SupervisorName);
            Assert.Equal(actual1.TimeCard, actual2.TimeCard);
            Assert.Equal(actual1.Type, actual2.Type);
        }

        [Fact]
        public void container_add_singleton_registration_with_initiator2()
        {
            var container = getContainer();
            var clid = 123456;
            var cid = 9696;
            var cname = "Test Company";
            var eid = Guid.NewGuid();
            var ename = "John Doe";
            var cevent = DateTime.Now;
            var sid = Guid.NewGuid(); ;
            var sname = "Matt Bastage";
            var tc = DateTime.Now;
            var tcevent = ClockEventTypes.LunchEnd;

            container.AddSingleton<ClockEvent>(x => new ClockEvent
            {
                ClockId = clid,
                CompanyId = cid,
                CompanyName = cname,
                EmployeeId = eid,
                EmployeeName = ename,
                Event = cevent,
                SupervisorId = sid,
                SupervisorName = sname,
                TimeCard = tc,
                Type = tcevent,
            });

            var actual1 = container.Get<ClockEvent>();
            var actual2 = container.Get<ClockEvent>();

            Assert.Equal(clid, actual1.ClockId);
            Assert.Equal(cid, actual1.CompanyId);
            Assert.Equal(cname, actual1.CompanyName);
            Assert.Equal(eid, actual1.EmployeeId);
            Assert.Equal(ename, actual1.EmployeeName);
            Assert.Equal(cevent, actual1.Event);
            Assert.Equal(sid, actual1.SupervisorId);
            Assert.Equal(sname, actual1.SupervisorName);
            Assert.Equal(tc, actual1.TimeCard);
            Assert.Equal(tcevent, actual1.Type);

            Assert.Equal(actual1.ClockId, actual2.ClockId);
            Assert.Equal(actual1.CompanyId, actual2.CompanyId);
            Assert.Equal(actual1.CompanyName, actual2.CompanyName);
            Assert.Equal(actual1.EmployeeId, actual2.EmployeeId);
            Assert.Equal(actual1.EmployeeName, actual2.EmployeeName);
            Assert.Equal(actual1.Event, actual2.Event);
            Assert.Equal(actual1.SupervisorId, actual2.SupervisorId);
            Assert.Equal(actual1.SupervisorName, actual2.SupervisorName);
            Assert.Equal(actual1.TimeCard, actual2.TimeCard);
            Assert.Equal(actual1.Type, actual2.Type);
        }

        [Fact]
        public void container_add_singleton_registration_with_object1()
        {
            var container = getContainer();
            var target = getTestObj();

            container.AddSingleton<IClockEvent, ClockEvent>(target);

            var actual1 = container.Get<IClockEvent>();
            var actual2 = container.Get<IClockEvent>();

            Assert.Equal(target, actual2);

            Assert.Equal(actual1, actual2);

        }

        [Fact]
        public async void container_add_thread_async()
        {
            var container = getContainer();
            container.AddThreadAsync<IClockEvent, ClockEvent>();

            container.SetContextId();
            var target = container.Get<IClockEvent>();

            await AsynchronusA(container).ConfigureAwait(false);

            var actual = container.Get<IClockEvent>();

            container.ClearContextId();

            Assert.Equal(10, actual.ClockId);
        }

        [Fact]
        public async void container_add_two_threads_async()
        {
            var container = getContainer();
            container.AddThreadAsync<IClockEvent, ClockEvent>();

            var actual1 = container.GetThreadAsyncService<IClockEvent>();
            var actual2 = container.GetThreadAsyncService<IClockEvent>();

            var t1 = AsynchronusB(actual1);
            var t2 = AsynchronusC(actual2);

            await t1;
            await t2;

            Assert.Equal(5, actual1.ClockId);
            Assert.Equal(10, actual2.ClockId);
        }

        [Fact]
        public async void container_add_two_threads_crazy_async()
        {
            var container = getContainer();
            container.AddThreadAsync<IClockEvent, ClockEvent>();

            var actual1 = container.GetThreadAsyncService<IClockEvent>();

            var t1 = AsynchronusC(actual1).ConfigureAwait(false);

            container.SetContextId();
            var t2 = AsynchronusD(container).ConfigureAwait(false);
            var actual2 = container.Get<IClockEvent>();
            
            await t1;
            await t2;

            container.ClearContextId();

            Assert.Equal(10, actual1.ClockId);
            Assert.Equal(700, actual2.ClockId);
        }

        private async Task AsynchronusA(Container container)
        {
            var target = container.Get<IClockEvent>();

            for (var i = 1; i < 11; i++)
            {
                target.ClockId = i;

                await Task.Delay(100);

                Assert.Equal(i, target.ClockId);
            }
        }

        private async Task<bool> AsynchronusB(IClockEvent clockEvent)
        {
            for (var i = 1; i < 11; i++)
            {
                clockEvent.ClockId = 5;

                await Task.Delay(100);

                Assert.Equal(5, clockEvent.ClockId);
            }

            return true;
        }

        private async Task<bool> AsynchronusC(IClockEvent clockEvent)
        {
            for (var i = 1; i < 11; i++)
            {
                clockEvent.ClockId = i;

                await Task.Delay(100);

                Assert.Equal(i, clockEvent.ClockId);
            }

            return true;
        }

        private async Task<bool> AsynchronusD(Container container)
        {
            for (var i = 1; i < 11; i++)
            {
                var target = container.Get<IClockEvent>();

                target.ClockId *= 2;

                var tranform = AsynchronusE(container, target);

                await Task.Delay(100);
                await tranform;

                var target2 = container.Get<IClockEvent>();

                Assert.Equal(target.ClockId, target2.ClockId);
            }

            return true;
        }

        private async Task<bool> AsynchronusE(Container container, IClockEvent clockEvent)
        {
            for (var i = 1; i < 11; i++)
            {
                var target = container.Get<IClockEvent>();

                target.ClockId = i * 7;
                
                await Task.Delay(100);

                clockEvent.ClockId *= 10;

                var target2 = container.Get<IClockEvent>();

                Assert.Equal(target.ClockId, target2.ClockId);
                Assert.Equal(clockEvent.ClockId, target2.ClockId);
            }

            return true;
        }
    }
}

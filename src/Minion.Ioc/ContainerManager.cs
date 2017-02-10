using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Minion.Ioc.Profiler;
using Minion.Ioc.Builders;

namespace Minion.Ioc
{
    public static class ContainerManager
    {
        private static readonly ConcurrentDictionary<string, Container> Containers;
        private static readonly LoggerFactory Factory;
        private static readonly string NameSpace;

        private const string Default = "DefaultContainer";

        static ContainerManager()
        {
            Factory = new LoggerFactory();
            NameSpace = typeof (Container).FullName;

            Containers = new ConcurrentDictionary<string, Container>();
        }

        public static Container GetContainer(string key = Default)
        {
            var container = Containers.GetOrAdd(key, x =>
            {
                var log = Factory.CreateLogger($"{key} {NameSpace}");
                var profiler = new DependencyProfiler(log);
                var builder = new DepedencyResolver(log, profiler);

                return new Container(log, profiler, builder);
            });

            return container;
        }
    }
}

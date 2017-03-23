using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Minion.Ioc.Profiler;
using Minion.Ioc.Builders;
using Minion.Ioc.Aspects;
using Minion.Ioc.Interfaces;

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
            NameSpace = typeof(Container).FullName;

            Containers = new ConcurrentDictionary<string, Container>();
        }

        public static Container GetContainer(string containerName = null)
        {
            containerName = string.IsNullOrWhiteSpace(containerName) ? Default : containerName;

            var container = Containers.GetOrAdd(containerName, x =>
            {
                var log = Factory.CreateLogger($"{containerName} {NameSpace}");
                var emitter = new AspectEmitter();
                var cache = new TypeCache(emitter);
                var profiler = new DependencyProfiler(log, new ConstructorProfiler(cache));
                var builder = new DepedencyResolver(log, profiler);

                return new Container(log, profiler, builder);
            });

            return container;
        }
    }
}

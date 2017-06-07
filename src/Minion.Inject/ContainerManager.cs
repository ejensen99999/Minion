using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Minion.Inject.Profiling;

namespace Minion.Inject
{
    public class ContainerManager
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
			    //var emitter = new AspectEmitter();
			    //var cache = new TypeCache(emitter);
			    //var profiler = new DependencyProfiler(log, new ConstructorProfiler(cache));
			    //var builder = new DepedencyResolver(log, profiler);

			    return new Container(new Profiler());
		    });

		    return container;
	    }
	}
}

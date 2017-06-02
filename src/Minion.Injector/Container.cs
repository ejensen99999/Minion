using Minion.Injector.Exceptions;
using Minion.Injector.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Minion.Core;
using Minion.Injector.Profiling;

namespace Minion.Injector
{
	public class Container : IServiceProvider
	{
		private readonly object _synclock;
		private readonly ConcurrentDictionary<string, ITypeBuilder> _builders;
		private readonly AsyncLocal<Guid> _contextId;
		private readonly IProfiler _profiler;

		public Guid ContextId
		{
			get { return _contextId.Value; }
		}

		public Container(IProfiler profiler)
		{
			_synclock = new object();
			_builders = new ConcurrentDictionary<string, ITypeBuilder>();
			_contextId = new AsyncLocal<Guid>();
			_profiler = profiler;
		}

		public void AddService(Type contract,
			Type concrete,
			ServiceLifetime lifeCycle,
			Func<Container, object> initializer,
			dynamic instance)
		{
			var success = ThreadControl.DoubleLock(_synclock, 
				() => _builders.DoesNotContainKey(contract),
				() => AddBuilder(contract, concrete, lifeCycle, initializer, instance));

			if (!success)
			{
				throw new IocRegistrationException(
					$"Type {contract.FullName}: has already been registered with this Ioc container");
			}
		}

		private void AddBuilder(Type contract,
			Type concrete,
			ServiceLifetime lifeCycle,
			Func<Container, dynamic> initializer,
			dynamic instance)
		{
			var ctor = _profiler.GetOptimalConstructor(concrete);
			var profile = new Profile(contract, concrete, lifeCycle, ctor, initializer, instance);
			var builder = _profiler.GetTypeBuilder(profile, lifeCycle);

			_builders.TryAdd(contract, builder);
		}

		public object GetService(Type serviceType)
		{
			var output = default(object);

			try
			{
				var builder = _builders.TryGet(serviceType);
				output = builder.Build(this);
			}
			catch (Exception ex)
			{
				throw new IocRetrievalException($"Could not resolve object: {serviceType.FullName}", ex);
			}

			return output;
		}

		public Container ClearContextId()
		{
			_builders.All(x => x.Value.Clean(_contextId.Value));
			_contextId.Value = new Guid();

			return this;
		}

		public Container SetContextId()
		{
			_contextId.Value = Guid.NewGuid();

			return this;
		}

	}
}

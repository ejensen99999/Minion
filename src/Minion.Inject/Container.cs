﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Minion.Core;
using Minion.Inject.Emit;
using Minion.Inject.Exceptions;
using Minion.Inject.Interfaces;
using Minion.Inject.Profiling;

namespace Minion.Inject
{
	public class Container : IServiceProvider
	{
		private readonly object _synclock;
		private readonly ConcurrentDictionary<string, ITypeBuilder> _builders;
		private readonly AsyncLocal<Guid> _contextId;
		private readonly IProfiler _profiler;

		private void AddBuilder(Type contract,
			Type concrete,
			ServiceLifetime lifeCycle,
			Func<Container, dynamic> initializer,
			dynamic instance)
		{
			var ctor = _profiler.GetOptimalConstructor(concrete);
		    var parameters = _profiler.GetParameters(ctor);
		    var constructor = ConstructorEmitter.Emit(concrete, ctor, parameters);
            var profile = new Profile(contract, lifeCycle, parameters, constructor, initializer, instance);
			var builder = _profiler.GetTypeBuilder(profile, lifeCycle);

			_builders.TryAdd(contract, builder);
		}

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

		public object GetService(Type serviceType)
		{
			var output = default(object);

			try
			{
			    if (serviceType.Equals(typeof(Container)))
			    {
			        output = this;
			    }
			    else
			    {
			        var builder = _builders.TryGet(serviceType);
			        output = builder.Build(this);
			    }
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

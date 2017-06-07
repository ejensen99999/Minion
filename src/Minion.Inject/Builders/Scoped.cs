using System;
using System.Collections.Concurrent;
using Minion.Inject.Interfaces;
using Minion.Inject.Profiling;

namespace Minion.Inject.Builders
{
	public class Scoped : Transient, ITypeBuilder
	{
		private readonly ConcurrentDictionary<Guid, dynamic> _threadInstances;

		public Scoped(Profile profile)
			: base(profile)
		{
			_threadInstances = new ConcurrentDictionary<Guid, dynamic>();
		}

		public override dynamic Build(Container container)
		{
			dynamic output = _threadInstances.GetOrAdd(container.ContextId, x =>
			{
				return base.Build(container);
			});

			return output;
		}

		public override bool Clean(Guid contextId)
		{
			dynamic garbage;
			return _threadInstances.TryRemove(contextId, out garbage);
		}
	}
}

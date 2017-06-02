using Minion.Injector.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Minion.Injector.Profiling;

namespace Minion.Injector.Builders
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

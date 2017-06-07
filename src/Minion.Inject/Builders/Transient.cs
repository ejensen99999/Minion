using System;
using Minion.Inject.Interfaces;
using Minion.Inject.Profiling;

namespace Minion.Inject.Builders
{
	public class Transient : BaseBuilder, ITypeBuilder
	{
		public Transient(Profile profile)
			: base(profile)
		{
		}

		public virtual dynamic Build(Container container)
		{
			var output = MaterializeObject(container, _profile);

			return output;
		}

		public virtual bool Clean(Guid contextId)
		{
			return true;
		}
	}
}

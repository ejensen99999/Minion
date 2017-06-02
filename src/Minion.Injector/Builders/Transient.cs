using System;
using System.Collections.Generic;
using System.Text;
using Minion.Injector.Interfaces;
using Minion.Injector.Profiling;

namespace Minion.Injector.Builders
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

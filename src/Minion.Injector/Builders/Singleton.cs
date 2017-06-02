using System;
using Minion.Injector.Interfaces;
using Minion.Injector.Profiling;

namespace Minion.Injector.Builders
{
	public class Singleton : BaseBuilder, ITypeBuilder
	{
		public Singleton(Profile profile)
			: base(profile)
		{
		}

		public dynamic Build(Container container)
		{
			if (_profile.Instance == null)
			{
				lock (_synclock)
				{
					if (_profile.Instance == null)
					{
						_profile.Instance = MaterializeObject(container, _profile);
					}
				}
			}

			return _profile.Instance;
		}

		public bool Clean(Guid contextId)
		{
			return true;
		}
	}
}

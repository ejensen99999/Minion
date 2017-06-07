using System;
using Minion.Inject.Interfaces;
using Minion.Inject.Profiling;

namespace Minion.Inject.Builders
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

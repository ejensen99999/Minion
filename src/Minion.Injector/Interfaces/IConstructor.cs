using System.Collections.Generic;

namespace Minion.Injector.Profiling
{
	public interface IConstructor
	{
		object Construct(List<object> parameters);
	}
}
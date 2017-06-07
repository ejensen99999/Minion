using System.Collections.Generic;

namespace Minion.Inject.Interfaces
{
	public interface IConstructor
	{
	    object Construct(List<object> parameters);
	}
}
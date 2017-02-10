using System;

namespace Minion.ServiceModel
{
	public interface IChannelProxy<T> : IDisposable
	{
		T Operations
		{
			get;
		}

		T SetHeaderContainer<V>(V container);
	}
}
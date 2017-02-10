using System.Collections.Concurrent;

namespace Minion.Messaging
{
	public class Index<T,V> where V: new()
	{
		private ConcurrentDictionary<T, V> _indices = new ConcurrentDictionary<T, V>();

		public V this[T input]
		{
			get
			{
				V output = _indices.GetOrAdd(input, x => new V());

				return output;
			}
			set
			{
				_indices.TryAdd(input, value);
			}
		}

		public ConcurrentDictionary<T, V> Indices
		{
			get
			{
				return _indices;
			}
			set
			{
				_indices = value;
			}
		}
	}
}

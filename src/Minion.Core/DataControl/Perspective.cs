using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Minion.DataControl
{
	public class Perspective<TData>
	{
		private readonly int _hash;
		private readonly List<DimensionIdentifier<TData>> _identifiers;
		private readonly ConcurrentDictionary<long, List<TData>> _slice;

		private long GetValueNumHash(TData item)
		{
			long hash = 0;

			for (var i = 0; i < _identifiers.Count; i++)
			{
				var ident = _identifiers[i];
				var val = ident.Identifier(item);
				var valHash = val.GetHashCode();

				hash += valHash << Globals.Primes[i];
			}

			return hash;
		}

		public Perspective(int hash, List<DimensionIdentifier<TData>> identifiers)
		{
			_hash = hash;
			_identifiers = identifiers;
			_slice = new ConcurrentDictionary<long, List<TData>>();
		}

		public void Load(TData item)
		{
			var sliceHash = GetValueNumHash(item);
		    List<TData> slice = _slice.GetOrAdd(sliceHash, x =>
		    {
                return new List<TData>();
		    });

			slice.Add(item);
		}

		public void Prune(TData item)
		{
			var sliceHash = GetValueNumHash(item);

			List<TData> garbage;
			_slice.TryRemove(sliceHash, out garbage);
		}

	    public List<TData> Slice(TData item)
	    {
	        List<TData> output;

	        var sliceHash = GetValueNumHash(item);
	        if (!_slice.TryGetValue(sliceHash, out output))
	        {
	            output = new List<TData>();
	        }

            return output.Where(x =>
            {
                return _identifiers.All(id => id.Identifier(x) == id.Identifier(item));
            }).ToList();
        }
	}
}

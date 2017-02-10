using System;
using System.Collections.Generic;
using System.Linq;
using Minion.DataControl.Interfaces;

namespace Minion.DataControl
{
	public class IndiceStore<TData> : IIndiceStore, IIndice
	    where TData : class, new()
	{
		private readonly IIndice _my;
		private readonly object _syncLock = new object();
		private List<TData> _indices;
		private Func<TData, bool> _dataFilter;
		private bool IsFiltered(TData item)
		{
			return (_dataFilter == null || _dataFilter(item)) && !((IIndice)this).Blank;
		}

		public IndiceStore()
		{
			_my = this;
			_indices = new List<TData>();
		}

		public IndiceStore(Func<TData, bool> dataFilter)
		    : this()
		{
			_dataFilter = dataFilter;
		}

		public TData this[int key]
		{
			get
			{
				lock (_syncLock)
				{
					var output = default(TData);
					var s = _indices.ElementAt(key);

					if (s != null)
					{
						output = s;
					}

					return output;
				}
			}

			set
			{
				if (IsFiltered(value))
				{
					lock (_syncLock)
					{
						if (_indices.ElementAt(key) == null)
						{
							Add(value);
						}
						else
						{
							_indices[key] = value;
						}
					}
				}
			}
		}

		dynamic IIndice.Id { get; set; }
		IIndice IIndice.Parent { get; set; }
		bool IIndice.Blank { get; set; }

		public bool IsIndexed(int key)
		{
			var test = _indices.ElementAt(key);
			return test != null;
		}

		public void Add(TData item)
		{
			if (IsFiltered(item))
			{
				_indices.Add(item);
			}
		}

		public void AddRange(IEnumerable<TData> data)
		{
			foreach (var datum in data)
			{
				Add(datum);
			}
		}

		public void AddRange(TData[] data)
		{
			foreach (var datum in data)
			{
				Add(datum);
			}
		}

		public void Clean()
		{
			lock (_syncLock)
			{
				_indices = _indices.Where(x => IsFiltered(x)).ToList();
			}
		}

		public IIndice Find(params dynamic[] keys)
		{
			//not yet implemented
			return null;
		}

		public IIndice Prune()
		{
			return this;
		}

		public IEnumerable<dynamic> Slice()
		{
			return _indices;
		}

		IIndice IIndice.Prune(dynamic key)
		{
			return null;
		}

		void IIndice.SetBlank()
		{
			_my.Blank = true;
		}

		void IIndice.SetParentAndId(IIndice parent, dynamic id)
		{
			if (_my.Parent == null)
			{
				_my.Parent = parent;
				_my.Id = id;
			}
		}
	}
}
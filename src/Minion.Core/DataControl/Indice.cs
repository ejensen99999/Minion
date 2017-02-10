using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minion.DataControl.Interfaces;

namespace Minion.DataControl
{
	public class Indice<TKey, TData> : IIndice
	    where TData : new()
	{
		#region Private Members

		private readonly IIndice _my;
		private static TData _default = new TData();
		private Func<TKey, bool> _keyFilter;
		private ConcurrentDictionary<TKey, TData> _indices;
		
		private bool IsFiltered(TKey key)
		{
			return _keyFilter == null || _keyFilter(key);
		}

		private void BlankIt(TData data)
		{
			if (data is IIndice && !((IIndice)data).Blank)
			{
				((IIndice)data).SetBlank();
			}
		}

		#endregion

		public Indice()
		{
			_my = this;
			_indices = new ConcurrentDictionary<TKey, TData>();
		}

		public Indice(Func<TKey, bool> keyFilter)
		    : this()
		{
			_keyFilter = keyFilter;
		}

		public TData this[TKey key]
		{
			get
			{
				var output = _default;
				BlankIt(output);

				if (!_my.Blank)
				{
					if (IsFiltered(key))
					{
					    output = _indices.GetOrAdd(key, x =>
					    {
					        var op = new TData();

					        (op as IIndice)?.SetParentAndId(this, key);

					        return op;
					    });
					}
				}
				return output;
			}

			set { _indices.AddOrUpdate(key, value, (x, y) => value); }
		}

		dynamic IIndice.Id { get; set; }
		IIndice IIndice.Parent { get; set; }
		bool IIndice.Blank { get; set; }

		public ConcurrentDictionary<TKey, TData> Indices
		{
			get { return _indices; }
		}

		public bool IsIndexed(TKey key)
		{
			return _indices.ContainsKey(key);
		}

		public void Clean()
		{
			foreach (var indice in _indices)
			{
				var key = indice.Key;
				var data = indice.Value;

				if (!IsFiltered(key))
				{
					TData garbage;
					_indices.TryRemove(key, out garbage);
				}
				else if (data is IIndice)
				{
					((IIndice)data).Clean();
				}
			}
		}

		public IIndice Find(params dynamic[] keys)
		{
			dynamic node = this;

			foreach (var key in keys)
			{
				var working = node[key];

				if (working is IIndiceStore)
				{
					break;
				}

				node = working;
			}

			return node;
		}

		public IIndice Prune()
		{
			return _my.Parent.Prune(_my.Id);
		}

		public IEnumerable<dynamic> Slice()
		{
			var output = new List<dynamic>();

			foreach (var indice in _indices.Values)
			{
				if (indice is IIndice)
				{
					output.AddRange(((IIndice)indice).Slice());
				}
				else
				{
					output.Add(indice);
				}
			}

			return output;
		}

		IIndice IIndice.Prune(dynamic key)
		{
			TData output;
			_indices.TryRemove(key, out output);

			return output as IIndice;
		}

		void IIndice.SetBlank()
		{
			((IIndice)this).Blank = true;
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

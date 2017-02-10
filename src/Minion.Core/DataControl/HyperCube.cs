using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Minion.DataControl
{
	public class HyperCube<TData>
	{
		private readonly Dictionary<string, DimensionIdentifier<TData>> _dimensions;
		private readonly Dictionary<int, Perspective<TData>> _perspectives;
		private readonly List<TData> _data;

		private void CreatePerspectives(List<DimensionIdentifier<TData>> parents, List<DimensionIdentifier<TData>> dimensions)
		{
			foreach (var dim in dimensions)
			{
				if (!parents.Contains(dim))
				{
					var newParents = new List<DimensionIdentifier<TData>>(parents);
					newParents.Add(dim);

					var perspectiveHash = GetPerspectiveHash(newParents);
					var pers = new Perspective<TData>(perspectiveHash, newParents);

					if (!_perspectives.ContainsKey(perspectiveHash))
					{
						_perspectives.Add(perspectiveHash, pers);
					}

					CreatePerspectives(newParents, dimensions);
				}
			}
		}

		private IEnumerable<DimensionIdentifier<TData>> GetLoadedIdentifiers(Expression<Func<TData, dynamic>>[] expressions)
		{
			var output = new List<DimensionIdentifier<TData>>();

			foreach (var expression in expressions)
			{
				var name = expression.ToFieldIdentifier();

				DimensionIdentifier<TData> identifier;
				if (!_dimensions.TryGetValue(name, out identifier))
				{
					throw new ArgumentException($"Property: {name} is not part of the cube dimensioning");
				}
				output.Add(identifier);
			}

			return output;
		}

		private IEnumerable<DimensionIdentifier<TData>> GetIdentifiers(Expression<Func<TData, dynamic>>[] expressions)
		{
			var shift = 0;

			return expressions.Select(expression => GetIdentifier(expression, shift++));
		}

		private DimensionIdentifier<TData> GetIdentifier(Expression<Func<TData, dynamic>> expression, int shift)
		{
			var lambda = expression.Compile();
			var name = expression.ToFieldIdentifier();

			var did = new DimensionIdentifier<TData>
			{
				Identifier = lambda,
				Name = name,
				Shift = 1 << shift
			};

			return did;
		}


		private int GetPerspectiveHash(IEnumerable<DimensionIdentifier<TData>> dimensions)
		{
			int output = 0;

			foreach (var dimension in dimensions)
			{
				output += dimension.Shift;
			}

			return output;
		}

		private HyperCube()
		{
			_dimensions = new Dictionary<string, DimensionIdentifier<TData>>();
			_perspectives = new Dictionary<int, Perspective<TData>>();
			_data = new List<TData>();
		}

		public HyperCube(params Expression<Func<TData, dynamic>>[] expressions)
			: this()
		{
			if (expressions.Length > 7)
			{
				throw new ArgumentException("Seriously? More than seven dimensions is ridiculous");
			}

			var dids = GetIdentifiers(expressions);

			foreach (var did in dids)
			{
				_dimensions.Add(did.Name, did);
			}

			CreatePerspectives(new List<DimensionIdentifier<TData>>(), _dimensions.Values.ToList());
		}

		public void Load(TData item)
		{
			foreach (var perspective in _perspectives.Values)
			{
				perspective.Load(item);
			}
		}

		public void Prune(TData item)
		{
			foreach (var perspective in _perspectives.Values)
			{
				perspective.Prune(item);
			}
		}

	    public List<TData> Slice(TData item, params Expression<Func<TData, dynamic>>[] searchParameters)
		{
			var output = new List<TData>();
			var dids = GetLoadedIdentifiers(searchParameters);
			var perspectiveHash = GetPerspectiveHash(dids);

			Perspective<TData> perspective;
			if (_perspectives.TryGetValue(perspectiveHash, out perspective))
			{
                output = perspective.Slice(item);
			}

			return output;
		}
	}

	public class DimensionIdentifier<TData>
	{
		public Func<TData, dynamic> Identifier { get; set; }
		public string Name { get; set; }
		public int Shift { get; set; }
	}
}

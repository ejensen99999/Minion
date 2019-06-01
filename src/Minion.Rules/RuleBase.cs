using System.Collections.Generic;
using Minion.Rules.Components;

namespace Minion.Rules
{
	public class RuleBase<X,Y>
	{
		private readonly IList<Criteria<X, Y>> _criteria;

		public RuleBase()
		{
			_criteria = new List<Criteria<X, Y>>();
		}

		public Criteria<X, Y> AddCriteria()
		{
			var crit = new Criteria<X, Y>();
			_criteria.Add(crit);
			return crit;
		}

		public Criteria<X, Y> NewCriteria()
		{
			return new Criteria<X, Y>();
		}

		public TriState EvaluateCriteria(X specification, Y facts, bool execute = false)
		{
			if (_criteria.Count == 0)
				return TriState.True;
			
			foreach (var i in _criteria)
			{
				var match = i.Match(specification, facts, execute);
				if (match != TriState.False)
					return match;
			}

			return TriState.False;
		}
	}
}

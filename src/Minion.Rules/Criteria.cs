using System;
using System.Collections;
using System.Collections.Generic;
using Minion.Rules.Components;

namespace Minion.Rules
{
	public class Criteria<X, Y> : IEnumerable
	{
		private List<Func<X, Y, bool>> _conditions = new List<Func<X, Y, bool>>();
		private Action<X, Y> _action;
		private TriState? _state;

		public IEnumerator GetEnumerator()
		{
			return null;
		}

		public Criteria<X, Y> Action(Action<X, Y> action)
		{
			_action = action;

			return this;
		}

		public Criteria<X, Y> Condition(Func<X, Y, bool> condition)
		{
			_conditions.Add(condition);

			return this;
		}

		public void Fuzzy()
		{
			_state = TriState.Maybe;
		}

		public void Clear()
		{
			_conditions = new List<Func<X, Y, bool>>();
			_action = null;
		}

		public TriState Match(X specification, Y facts, bool execute)
		{
			foreach (var condition in _conditions)
			{
				if (!condition(specification, facts))
					return TriState.False;
			}

			if (execute && _action != null)
			{
				_action(specification, facts);
			}

			return _state.HasValue ? _state.Value : TriState.True;
		}
	}
}

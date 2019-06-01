using System.Collections.Generic;
using Minion.Rules.Components;
using Minion.Rules.Interfaces;

namespace Minion.Rules
{
	public static class EngineHelpers
	{
		public static void AddSuccessfulRule(this IRuleSpecification specification, object rule)
		{
			if (specification.SuccessfulIndexes == null)
				specification.SuccessfulIndexes = new HashSet<string>();

			specification.SuccessfulIndexes.Add(rule.ToString());
		}

		public static bool ToBool(this TriState state)
		{
			return state == TriState.True;
		}

		public static TriState IsFuzzy(this TriState state, TriState newState)
		{
			var output = TriState.False;

			if (state == TriState.False || newState == TriState.False)
			{
				output = state;
			}
			else if (state == TriState.Maybe)
			{
				output = TriState.Maybe;
			}
			else
			{
				output = newState;
			}

			return output;
		}
	}
}

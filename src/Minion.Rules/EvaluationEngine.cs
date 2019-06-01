using System;
using System.Collections.Generic;
using Minion.Rules.Components;
using Minion.Rules.Interfaces;

namespace Minion.Rules
{
	public sealed class EvaluationEngine<T, V>
		where T : IRuleSpecification
		where V : IRuleFacts
	{
		private static object _syncRoot = new Dictionary<T, V>();
		private static volatile EvaluationEngine<T, V> _instance;
		private readonly Dictionary<Enum, RuleBase<T, V>[]> _rules;

		public static EvaluationEngine<T, V> Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_syncRoot)
					{
						if (_instance == null)
						{
							_instance = new EvaluationEngine<T, V>();
						}
					}
				}
				return _instance;
			}
		}

		private EvaluationEngine()
		{
			_rules = new Dictionary<Enum, RuleBase<T, V>[]>();
		}

		public void ClearRules()
		{
			_rules.Clear();
		}

		public RuleAgenda<T, V> CreateAgenda()
		{
			return new RuleAgenda<T, V>
			{
				RuleChaining = ChainType.All,
				SpecificationChaining = ChainType.Any
			};
		}

		public RuleAgenda<T, V> CreateAgenda(T specification, V facts)
		{
			return CreateAgenda()
				.AddSpecification(specification)
				.SetFact(facts);
		}

		public RuleAgenda<T, V> CreateAgenda(IEnumerable<T> specifications, V facts)
		{
			var agenda = CreateAgenda()
				.SetFact(facts);

			foreach (var i in specifications)
			{
				agenda.AddSpecification(i);
			}

			return agenda;
		}

		public bool Execute(RuleAgenda<T, V> agenda)
		{
			return EvaluateSpecifications(agenda, true).ToBool();
		}

		public bool Evaluate(RuleAgenda<T, V> agenda)
		{
			return EvaluateSpecifications(agenda, false).ToBool();
		}

		public EvaluationEngine<T, V> RegisterRule(Enum index, params RuleBase<T, V>[] ruleIndexes)
		{
			_rules.Add(index, ruleIndexes);
			return _instance;
		}

		public bool Resolve(RuleAgenda<T, V> agenda)
		{
			var resolved = false;
			foreach (T specification in agenda.Specifications)
			{
				if (specification.Successful == TriState.True)
				{
					var modified = ResolveFor(specification, (V)agenda.Facts);

					if (modified)
						resolved = true;
				}
			}
			return resolved;
		}

		public bool ResolveFor(T specification, V facts)
		{
			if (specification.Successful != TriState.True)
			{
				throw new ArgumentException(
					"Only pre-evaluated, successful rule specifications can be resolved. Please evaluate your agenda first");
			}

			RuleBase<T, V>[] rules;
			var registered = _rules.TryGetValue(specification.ResolutionRuleIndex, out rules);

			if (registered)
			{
				facts.Modified = rules.Length <= 0 || EvaluateRules(specification, facts, rules, true).ToBool();
			}

			return facts.Modified;
		}

		private TriState EvaluateSpecifications(RuleAgenda<T, V> agenda, bool execute)
		{
			var match = TriState.False;
			foreach (T specification in agenda.Specifications)
			{
				RuleBase<T, V>[] rules;
				var registered = _rules.TryGetValue(specification.RuleIndex, out rules);
				var facts = (V)agenda.Facts;

				if (!registered)
				{
					continue;
				}

				var evaluation = EvaluateRules(specification, facts, rules, execute, agenda.RuleChaining);

				switch (agenda.SpecificationChaining)
				{
					case ChainType.All:
						if (evaluation != TriState.False)
						{
							specification.Successful = evaluation;
							match = evaluation;
						}
						else
						{
							return TriState.False;
						}
						break;

					case ChainType.Any:
						if (evaluation != TriState.False)
						{
							specification.Successful = evaluation;
							match = evaluation;
						}
						break;

					case ChainType.First:
						if (evaluation != TriState.False)
						{
							specification.Successful = evaluation;
							return evaluation;
						}
						break;
				}
			}
			return match;
		}

		public TriState EvaluateRules(
			T specification,
			V facts,
			IEnumerable<RuleBase<T, V>> rules,
			bool execute,
			ChainType chaining = ChainType.All)
		{
			var match = TriState.True;

			foreach (var rule in rules)
			{
				var evaluation = rule.EvaluateCriteria(specification, facts, execute);

				switch (chaining)
				{
					case ChainType.All:
						if (evaluation != TriState.False)
						{
							specification.AddSuccessfulRule(rule);
							match = match.IsFuzzy(evaluation);
						}
						else
						{
							return TriState.False;
						}
						break;

					case ChainType.Any:
						if (evaluation != TriState.False)
						{
							specification.AddSuccessfulRule(rule);
							match = match.IsFuzzy(evaluation);
						}
						break;

					case ChainType.First:
						if (evaluation != TriState.False)
						{
							specification.AddSuccessfulRule(rule);
							return evaluation;
						}
						break;
				}
			}
			return match;
		}
	}
}

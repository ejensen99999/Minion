using System.Collections.Generic;
using Minion.Rules.Components;
using Minion.Rules.Interfaces;

namespace Minion.Rules
{
	public class RuleAgenda<T, V>
		where T : IRuleSpecification
		where V : IRuleFacts
	{
		public bool Effected { get; set; }

		public List<IRuleSpecification> Specifications { get; set; }
		public IRuleFacts Facts { get; set; }
		public ChainType SpecificationChaining { get; set; }
		public ChainType RuleChaining { get; set; }

		public RuleAgenda()
		{
			Specifications = new List<IRuleSpecification>();
		}

		public RuleAgenda<T, V> AddSpecification(IRuleSpecification ruleSpecification)
		{
			Specifications.Add(ruleSpecification);
			return this;
		}

		public void ClearSpecification()
		{
			Specifications = new List<IRuleSpecification>();
		}

		public RuleAgenda<T, V> SetFact(IRuleFacts fact)
		{
			Facts = fact;
			return this;
		}
	}
}

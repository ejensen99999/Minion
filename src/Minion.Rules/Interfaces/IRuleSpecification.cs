using System;
using System.Collections.Generic;
using Minion.Rules.Components;

namespace Minion.Rules.Interfaces
{
	public interface IRuleSpecification
	{
		int SpecificationId { get; set; }
		string Name { get; set; }
		string Description { get; set; }
		string DisplayText { get; set; }
		
		Enum RuleFail { get; set; }
		Enum RuleIndex { get; set; }
		Enum ResolutionRuleIndex { get; set; }
		HashSet<string> SuccessfulIndexes { get; set; }
		TriState Successful { get; set; }
		
	}
}

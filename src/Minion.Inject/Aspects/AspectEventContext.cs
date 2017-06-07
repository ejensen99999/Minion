using System;

namespace Minion.Inject.Aspects
{
	public class AspectEventContext
	{
		public dynamic CallerReference { get; set; }
		public Type CallerType { get; set; }
		public string MemberName { get; set; }
	}
}

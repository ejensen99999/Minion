using System;

namespace Minion.Security
{
	public interface ISecurityAttribute
	{
		string Description
		{
			get;
		}

		Guid Id
		{
			get;
		}
	}
}
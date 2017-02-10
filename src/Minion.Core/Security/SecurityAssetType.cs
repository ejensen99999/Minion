using System;
using System.Runtime.Serialization;

namespace Minion.Security
{
	public enum SecurityAssetType
	{
		None = 0,
		Action = 1,
		Controller = 2,
		Report = 6,
		Admin = 10
	}
}
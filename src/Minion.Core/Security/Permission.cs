using System;

namespace Minion.Security
{
	public class Permission
	{
		public int AssetId
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public int PermissionType
		{
			get;
			set;
		}

		public Guid UniqueId
		{
			get;
			set;
		}
	}
}
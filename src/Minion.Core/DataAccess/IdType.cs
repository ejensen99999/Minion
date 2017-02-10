using System;

namespace Minion.DataAccess
{
	public class IdType
	{
		public int Id
		{
			get;
			set;
		}

		public string StringId
		{
			get;
			set;
		}

		public Guid UniqueId
		{
			get;
			set;
		}

		public IdType()
		{
		}
	}
}
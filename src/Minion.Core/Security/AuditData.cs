using System;

namespace Minion.Security
{
	public class AuditData
	{
		public string Assembly
		{
			get;
			set;
		}

		public DateTime Date
		{
			get;
			set;
		}

		public object Input
		{
			get;
			set;
		}

		public string Operation
		{
			get;
			set;
		}

		public object Output
		{
			get;
			set;
		}

		public string Service
		{
			get;
			set;
		}

		public string User
		{
			get;
			set;
		}
	}
}
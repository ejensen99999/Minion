using System;
using System.Reflection;

namespace Minion.DataAccess
{
	public class SqlColumn
	{
		public string Column
		{
			get;
			set;
		}

		public Type DataType
		{
			get;
			set;
		}

		public string DBColumn
		{
			get;
			set;
		}

		public string Parameter
		{
			get;
			set;
		}

		public PropertyInfo Property
		{
			get;
			set;
		}

		public SqlColumn()
		{
		}
	}
}
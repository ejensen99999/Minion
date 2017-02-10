using System.Collections.Generic;

namespace Minion.DataAccess
{
	public class SqlContainer
	{
		public Dictionary<string, SqlColumn> Columns
		{
			get;
			set;
		}

		public SqlColumn PrimaryKey
		{
			get;
			set;
		}

		public string SQL
		{
			get;
			set;
		}

		public SqlContainer()
		{
			Columns = new Dictionary<string, SqlColumn>();
		}
	}
}
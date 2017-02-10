namespace Minion.DataAccess
{
	public class Filter
	{
		public FilterComparators Comparator
		{
			get;
			set;
		}

		public string Field
		{
			get;
			set;
		}

		public dynamic Value
		{
			get;
			set;
		}

		public Filter()
		{
		}
	}
}
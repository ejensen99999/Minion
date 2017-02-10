using System.Collections.Generic;
using System.Linq;

namespace Minion.DataAccess
{
	public class DataResult<T>
	{
		public List<T> Data
		{
			get;
			set;
		}

		public int DataTotal
		{
			get;
			set;
		}

		public DataResult()
		{
			Data = new List<T>();
		}

		public DataResult(IEnumerable<T> data)
		{
			Data = new List<T>(data);
		}
	}
}
using System.Collections.Generic;

namespace Minion.DataAccess
{
	public class FormReturn
	{
		public FormCommand command { get; set; }
		public Dictionary<string, string> data { get; set; }
		public List<string> delta { get; set; }
	}

	public class FormReturn<T>
	{
		public FormCommand command { get; set; }
		public T data { get; set; }
		public List<string> delta { get; set; }
	}
}
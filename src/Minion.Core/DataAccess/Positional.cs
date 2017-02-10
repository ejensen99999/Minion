using System.Collections.Generic;

namespace Minion.DataAccess
{
	public class Positional
	{
		private readonly List<int> _positions;

		public Positional(List<int> positions)
		{
			_positions = positions;
		}

		public Positional(int[] positions)
		{
			_positions = new List<int>(positions);
		}

		public string[] Parse(string line)
		{
			List<string> strs = new List<string>();
			int count = _positions.Count;
			int item = 0;
			for (int i = 0; i < count; i++)
			{
				int num = item;
				item = item + _positions[i];
				int item1 = _positions[i];
				strs.Add(line.Substring(num, item1));
			}
			if (item < line.Length)
			{
				strs.Add(line.Substring(item));
			}
			return strs.ToArray();
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sFndCLIWrapper;

namespace Minion.CP.Abstraction
{
	public class Motor
	{
		public string Name { get; }
		public cliINode Node { get; }
		public int MoveCount { get; set; } = 0;
		public int WrapCount { get; set; } = 0;
		public int MoveTimeout { get; set; } = 0;
		public string ConfigFile { get; set; }

		public Motor(string name, cliINode node)
		{
			Name = name;
			Node = node;
		}
	}
}

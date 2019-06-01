using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minion.CP.Abstraction
{

	public class AxisStatusEventArgs : EventArgs
	{
		public string StatusMessage { get; }
		public NodeStatus Status { get; }
		public string Message { get; }

		public AxisStatusEventArgs(NodeStatus status, string message)
		{
			Status = status;
			Message = message;
		}
	}
}

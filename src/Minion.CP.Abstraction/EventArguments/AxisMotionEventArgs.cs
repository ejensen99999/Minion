using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minion.CP.Abstraction
{
	public class AxisMotionEventArgs : EventArgs
	{
		public Guid CorrelationId { get; }
		public bool Completed { get; }
		public string Status { get; }

		public AxisMotionEventArgs(Guid correlationId, bool completed, string status = null)
		{
			CorrelationId = correlationId;
			Completed = completed;
			Status = status;
		}
	}

}

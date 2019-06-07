using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minion.CP.Abstraction
{
	public enum NodeStatus
	{
		Enabling,
		Enabled,
		Disabling,
		Disabled,
		Homing,
		Homed,
		Moving,
		Moved,
		Faulted,
		Statistics
	}
}

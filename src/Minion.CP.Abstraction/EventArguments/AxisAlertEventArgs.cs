using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minion.CP.Abstraction
{
	public class AxisAlertEventArgs : EventArgs
	{
		public string NodeAddress { get; }
		public string[] Alerts { get; }

		public AxisAlertEventArgs(string address, string[] alerts)
		{
			NodeAddress = address;
			Alerts = alerts;
		}
	}
}

using System;
using Microsoft.Extensions.Logging;

namespace Minion.Configuration
{
	public class EmailSettings
	{
		public string DevEmail { get; set; }

		public string Host { get; set; }

		public TimeSpan Interval { get; set; } = TimeSpan.Parse("00:00:00.25");

		public int Port { get; set; }
	}
}
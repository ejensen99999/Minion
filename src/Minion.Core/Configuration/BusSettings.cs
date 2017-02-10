using System;
using Microsoft.Extensions.Logging;

namespace Minion.Configuration
{
	public class BusSettings
	{
		public TimeSpan DefaultTTL { get; set; }
	}
}
using System;
using Microsoft.Extensions.Logging;

namespace Minion.Configuration
{
	public class CacheSettings
	{
		public TimeSpan Duration { get; set; } = TimeSpan.Parse("00:10:00");
	}
}
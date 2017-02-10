using Microsoft.Extensions.Logging;

namespace Minion.Configuration
{
	public class CronSettings
	{
		public string ADSync { get; set; }

		public string CMSRetention { get; set; }
	}
}
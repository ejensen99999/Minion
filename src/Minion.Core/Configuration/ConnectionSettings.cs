using Microsoft.Extensions.Logging;

namespace Minion.Configuration
{
	public class ConnectionSettings
	{
		public string Auditing { get; set; }

		public string Bus { get; set; }

		public string Data { get; set; }

		public string Legacy { get; set; }

		public string Logging { get; set; }

		public string Security { get; set; }
	}
}
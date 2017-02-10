using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Minion.Configuration
{
	public class FileSettings
	{
		public string CFS { get; set; } = "D:\\CFS";

		public string Drop { get; set; } = "D:\\DROP";

		public string Templates { get; set; } = "D:\\TEMPLATES";

	}
}
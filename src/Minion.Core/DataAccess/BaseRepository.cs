using Microsoft.Extensions.Logging;

namespace Minion.DataAccess
{
	public class BaseRepository
	{
		protected readonly ILogger<CoreConfiguration> _log;

		protected readonly ICoreConfiguration _config;

		protected readonly ICommonCache _cc;

		public BaseRepository(ILogger<CoreConfiguration> Log, ICoreConfiguration config, ICommonCache cache)
		{
			_log = Log;
			_config = config;
			_cc = cache;
		}
	}
}
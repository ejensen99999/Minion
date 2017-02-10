using Microsoft.Extensions.Logging;
using Minion.Security;

namespace Minion
{
	public abstract class BaseLogic<T>
	{
	    protected readonly ILogger<T> _log;
		protected readonly ICoreConfiguration _config;
        protected readonly IUser _user;
		
		protected BaseLogic(ILoggerFactory log, ICoreConfiguration config, IUser user)
		{
		    _log = log.CreateLogger<T>();;
			_user = user;
			_config = config;
		}
	}
}
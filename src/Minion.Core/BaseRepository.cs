using Microsoft.Extensions.Logging;

namespace Minion
{
    public abstract class BaseRepository<T>
    {
        protected readonly ILogger<T> _log;
        protected readonly ICoreConfiguration _config;


        protected BaseRepository(ILoggerFactory log, ICoreConfiguration config)
        {
            _log = log.CreateLogger<T>();
            _config = config;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Minion.Ioc;

namespace Minion.Web.Models
{
    public interface IRespository
    {
        string GetId();
    }

    public class Respository
        : BaseRepository<Respository>,
            IRespository
    {
        private readonly Settings _settings;
        private readonly ITest _test;

        public Respository(ILoggerFactory log,
            ICoreConfiguration config,
            Settings settings,
            Container container,
            ITest test)
            : base(log, config)
        {
            _settings = settings;
            _config.Cache.Duration = TimeSpan.FromHours(1);
            _test = test;
            _test.Id = container.ContextId;
        }

        public string GetId()
        {
            return _test.Id.ToString();
        }

    }
}

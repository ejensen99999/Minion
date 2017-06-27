using System;
using Microsoft.Extensions.Logging;
using Minion.Inject;
using Minion.Inject.Aspects;
using Minion.Web.TestObjs;

namespace Minion.Web.Domain
{
    public interface IRespository
    {
        string GetId();
    }

    public class Repository
        : BaseRepository<Repository>,
            IRespository, IAspect
    {
        private readonly Settings _settings;
        private readonly ITest _test;

        public Repository(ILoggerFactory log,
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

        [MyMethodAspect(1)]
        [MyMethodAspect2(2)]
        public virtual string GetId()
        {
            return _test.Id.ToString();
        }
    }
}

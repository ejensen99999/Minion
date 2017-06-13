using System;
using Microsoft.Extensions.Logging;
using Minion.Web.TestObjs;
using Minion.Inject;
using Minion.Inject.Aspects;
using Minion.Core.ServiceModel;

namespace Minion.Web.Models
{
    public interface IRespository
    {
        string GetId();
    }

    public class Respository
        : BaseRepository<Respository>,
            IRespository, IAspect
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

        [MyMethodAspect(1)]
        [MyMethodAspect2(2)]
        public virtual string GetId()
        {
            return _test.Id.ToString();
        }
    }
}

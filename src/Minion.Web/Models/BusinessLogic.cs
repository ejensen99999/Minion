using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Minion.Web.Models
{
    public interface IBusinessLogic
    {
        string GetId();
    }

    public class BusinessLogic
        : BaseLogic<BusinessLogic>,
            IBusinessLogic
    {
        private readonly Settings _settings;
        private readonly IRespository _repository;
        private readonly ITest _test;

        public BusinessLogic(ILoggerFactory log,
            ICoreConfiguration config,
            Settings settings,
            IRespository repository,
            ITest test)
            : base(log, config, null)
        {
            _settings = settings;
            _repository = repository;
            _test = test;
        }

        public string GetId()
        {
            return _repository.GetId() + " | " + _test.Id.ToString();
        }
    }
}

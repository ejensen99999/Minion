﻿using Microsoft.Extensions.Logging;
using Minion.Web.TestObjs;

namespace Minion.Web.Domain
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

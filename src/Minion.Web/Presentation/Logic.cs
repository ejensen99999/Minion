using System;
using Microsoft.Extensions.Logging;
using Minion.Web.TestObjs;

namespace Minion.Web.Presentation
{
    public interface IPresentationLogic
    {
        string Validate();
        string Standardize();
        string Transform();
        string Condition();
        string Audit();
        string EventSource();
    }

    public class Logic
        : IPresentationLogic
    {
        private readonly Settings _settings;
        private readonly IPresentationRespository _repository;
        private readonly ITest _test;

        public Logic(ILoggerFactory log,
            IPresentationRespository repository)
        {
            _repository = repository;
        }

        public string Audit()
        {
            throw new NotImplementedException();
        }

        public string Condition()
        {
            throw new NotImplementedException();
        }

        public string EventSource()
        {
            throw new NotImplementedException();
        }

        public string Standardize()
        {
            throw new NotImplementedException();
        }

        public string Transform()
        {
            throw new NotImplementedException();
        }

        public string Validate()
        {
            throw new NotImplementedException();
        }
    }
}

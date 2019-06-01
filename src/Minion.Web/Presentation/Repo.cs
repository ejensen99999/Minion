using System;
using Microsoft.Extensions.Logging;
using Minion.Inject;
using Minion.Inject.Aspects;
using Minion.Web.Domain;
using Minion.Web.TestObjs;

namespace Minion.Web.Presentation
{
    public interface IPresentationRespository
    {
        string Validate(DateTime birthDate);
        //string Standardize(string );
        string Transform(DateTime birthDate);
        string Condition(DateTime birthDate);
        string Audit(TestRequest request);
        string EventSource(TestRequest request);
    }

    public class Repo : IPresentationRespository, IAspect
    {
        
        public Repo()
        {
        }

		public string Audit(TestRequest request)
		{
			throw new NotImplementedException();
		}

		public string Condition(DateTime birthDate)
		{
			throw new NotImplementedException();
		}

		public string EventSource(TestRequest request)
		{
			throw new NotImplementedException();
		}

		public string Transform(DateTime birthDate)
		{
			throw new NotImplementedException();
		}

		public string Validate(DateTime birthDate)
		{
			throw new NotImplementedException();
		}
	}
}

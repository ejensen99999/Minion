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

    }
}

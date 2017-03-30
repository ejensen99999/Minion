using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Minion.Ioc.Exceptions;
using Minion.Ioc.Interfaces;
using System.Reflection;
using System.Linq;

namespace Minion.Ioc.Builders
{
    public class DepedencyResolver : IDependencyResolver
    {
        private readonly ILogger _log;
        private readonly IDependencyProfiler _profiler;

        public DepedencyResolver(ILogger log, IDependencyProfiler profiler)
        {
            _log = log;
            _profiler = profiler;
        }

        public dynamic GetObject(Container container,
            Type contract)
        {
            var output = default(object);
            var builders = _profiler.Builders;

            var contractInfo = contract.GetTypeInfo();

            try
            {
                ITypeBuilder builder;
                if (builders.TryGetValue(contract, out builder))
                {
                    output = builder.Build(container);
                }
                else if (contract.Equals(typeof(Container)))
                {
                    output = container;
                }
                else if (contractInfo.IsGenericType)
                {
                    var t = 0;
                    var genArgs = contractInfo.GetGenericArguments();
                    var txt = string.Join(", ", genArgs.Select(x => x.AssemblyQualifiedName));
                    var ass = contractInfo.AssemblyQualifiedName;
                }
                else
                {
                    output = Activator.CreateInstance(contract);
                }
            }
            catch (Exception ex)
            {
                var message = "Could not retrieve Ioc object:" + contract.FullName;
                _log.LogError(message, ex);
                throw new IocRetrievalException(message, ex);
            }

            return output;
        }
    }
}

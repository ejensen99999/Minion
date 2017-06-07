using System.Collections.Generic;
using Minion.Inject.Exceptions;
using Minion.Inject.Profiling;

namespace Minion.Inject.Builders
{
    public class BaseBuilder
    {
        protected readonly object _synclock;
        protected readonly Profile _profile;

        public BaseBuilder( Profile profile)
        {
            _synclock = new object();
            _profile = profile;
        }

        protected dynamic MaterializeObject(Container container,
            Profile profile)
        {
            var parameterInstances = new List<dynamic>();
            var output = default(object);

            foreach (var parameter in profile.Parameters)
            {
                var param = container.GetService(parameter);

                if (param == null)
                {
                    throw new IocRetrievalException(
                        $"Could not materialize parameter for constructor: {parameter}");
                }

                parameterInstances.Add(param);
            }

            output = MaterializeType(container, profile, parameterInstances);

            return output;
        }

        protected dynamic MaterializeType(Container container,
            Profile profile,
            List<dynamic> parameters)
        {
            var output = profile.Initiate(container, parameters);

            return output;
        }
    }
}

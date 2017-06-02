using Minion.Injector.Exceptions;
using System.Collections.Generic;
using Minion.Injector.Profiling;

namespace Minion.Injector.Builders
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
            var output = default(object);

            if (profile.Concrete.Equals(typeof(Container)))
            {
                output = container;
            }
            else
            {
                output = profile.Initiate(container, parameters);
            }

            return output;
        }
    }
}

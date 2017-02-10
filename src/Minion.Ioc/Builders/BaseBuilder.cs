using System;
using System.Collections.Generic;
using Minion.Ioc.Exceptions;
using Minion.Ioc.Interfaces;
using Minion.Ioc.Models;

namespace Minion.Ioc.Builders
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
                var param = container.GetService(parameter.ContractType);

                if (param == null)
                {
                    throw new IocRetrievalException(
                        $"Could not materialize parameter for constructor: {parameter.ContractType}");
                }

                parameterInstances.Add(param);
            }

            output = MaterializeType(container, profile, parameterInstances);

            return output;
        }

        //protected dynamic MaterializeType(Container container,
        //    Profile profile,
        //    List<dynamic> parameters)
        //{
        //    var output = default(object);

        //    if (profile.Concrete.Equals(typeof(Container)))
        //    {
        //        output = container;
        //    }
        //    else
        //    {
        //        output = parameters == null || parameters.Count == 0
        //            ? Activator.CreateInstance(profile.Concrete)
        //            : Activator.CreateInstance(profile.Concrete, parameters.ToArray());
        //    }

        //    return output;
        //}

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
                output = profile.Construct(parameters);
            }

            return output;
        }
    }
}

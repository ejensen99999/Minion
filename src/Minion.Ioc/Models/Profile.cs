using System;
using System.Collections.Generic;
using Minion.Ioc.Interfaces;
using Microsoft.Extensions.Logging;

namespace Minion.Ioc.Models
{
    public class Profile: IProfile, IConstructor
    {
        private readonly IConstructor _constructor;

        public ILogger Log { get; set; }

        public Type Concrete { get; }

        public Type Contract { get; }

        public Lifetime Lifecycle { get;}

        public List<ParameterDefinition> Parameters { get; }

        public Func<Container, dynamic> Initializer { get; }

        public dynamic Instance { get; set; }

        public Profile(ILogger log,
            Type contract,
            Type concrete,
            Lifetime lifecycle,
            Func<Container, dynamic> initializer,
            ConstructorDefinition ctor)
        {
            Log = log;
            Concrete = concrete;
            Contract = contract;
            Lifecycle = lifecycle;
            Initializer = initializer;
            Parameters = ctor.Parameters;
            _constructor = ctor.Constructor;
        }

        public object Construct(List<object> parameters)
        {
            var output = _constructor.Construct(parameters);
            return output;
        }
    }
}
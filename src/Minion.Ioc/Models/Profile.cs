using System;
using System.Collections.Generic;
using Minion.Ioc.Interfaces;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Minion.Ioc.Models
{
    public class Profile
        : IProfile,
            IConstructor
    {
        private readonly IConstructor _constructor;

        public ILogger Log { get; set; }

        public Type Concrete { get; }

        public Type Contract { get; }

        public bool IsGeneric { get; }

        public Type[] GenericArguments { get; }

        public Lifetime Lifecycle { get;}

        public List<ParameterDefinition> Parameters { get; }

        public Func<Container, dynamic> Initializer { get; }

        public dynamic Instance { get; set; }

        public Profile(ILogger log,
            Type contract,
            Type concrete,
            Lifetime lifecycle,
            Func<Container, dynamic> initializer,
            ConstructorDefinition ctorDefinition)
        {
            Log = log;
            Concrete = concrete;
            Contract = contract;
            Lifecycle = lifecycle;
            Initializer = initializer;
            Parameters = ctorDefinition.Parameters;
            _constructor = ctorDefinition.Constructor;

            var info = concrete?.GetTypeInfo();

            IsGeneric = info != null && info.IsGenericType;
            GenericArguments = IsGeneric ? info.GetGenericArguments() : new Type[0];

        }

        public object Construct(List<object> parameters)
        {
            var output = _constructor.Construct(parameters);
            return output;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Minion.Inject.Interfaces;

namespace Minion.Inject.Profiling
{
	public class Profile : IProfile
	{
	    private readonly ConstructorInfo _ctor;

        public Type Concrete { get; }

		public Type Contract { get; }

		public ServiceLifetime Lifecycle { get; }

		public List<Type> Parameters { get; }

		public IConstructor Constructor { get; }

		public Func<Container, dynamic> Initializer { get; }

		public dynamic Instance { get; set; }

		public Profile(Type contract,
			Type concrete,
			ServiceLifetime lifeCycle,
			ConstructorInfo ctor,
			Func<Container, dynamic> initializer,
			dynamic instance)
		{
			Contract = contract;
			Concrete = concrete;
			Lifecycle = lifeCycle;
			Initializer = initializer;
			Instance = instance;

            _ctor = ctor;

		    if (ctor != null)
		    {
		        Parameters = ctor
		            .GetParameters()?
		            .Select(x => x.ParameterType)
		            .ToList();

		        Constructor = ConstructorEmitter.Emit(concrete, ctor, Parameters);
		    }
		}

		public object Initiate(Container container, List<object> parameters)
		{
			var output = new object();

            if (Instance != null)
            {
                output = Instance;
            }
            else if (Initializer != null)
            {
                output = Initializer(container);
            }
            else
            {
                output = Constructor.Construct(parameters); //225 ms/million
            }

            return output;
		}
	}
}

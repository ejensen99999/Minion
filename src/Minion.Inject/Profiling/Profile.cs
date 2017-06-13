using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Minion.Inject.Interfaces;
using Minion.Inject.Emit;

namespace Minion.Inject.Profiling
{
	public class Profile
	{
		public Type Contract { get; }

		public ServiceLifetime Lifecycle { get; }

	    public bool IsGeneric { get; }

	    public IEnumerable<Type> Parameters { get; }

		public IConstructor Constructor { get; }

		public Func<Container, dynamic> Initializer { get; }

		public dynamic Instance { get; set; }

		public Profile(Type contract,
			ServiceLifetime lifeCycle,
			IEnumerable<Type> parameters,
		    IConstructor constructor,
            Func<Container, dynamic> initializer,
			dynamic instance)
		{
			Contract = contract;
			Lifecycle = lifeCycle;
		    Parameters = parameters;
		    Constructor = constructor;
			Initializer = initializer;
			Instance = instance;

		    IsGeneric = contract.GetTypeInfo()
		        .IsGenericType;
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

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Linq;
using Minion.Injector.Interfaces;

namespace Minion.Injector.Profiling
{
	public class Profile : IProfile
	{
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

			Parameters = ctor
				.GetParameters()?
				.Select(x => x.ParameterType)
				.ToList();

			Constructor = ConstructorEmitter.Emit(concrete, ctor, Parameters);
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
				output = Constructor.Construct(parameters);
			}

			return output;
		}
	}
}

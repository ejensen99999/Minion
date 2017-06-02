using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Minion.Injector.Builders;
using Minion.Injector.Exceptions;
using Minion.Injector.Interfaces;

namespace Minion.Injector.Profiling
{
	public interface IProfiler
	{
		ConstructorInfo GetOptimalConstructor(Type concrete);

		ITypeBuilder GetTypeBuilder(Profile profile,
			ServiceLifetime lifeCycle);
	}

	public class Profiler: IProfiler
	{
		public ConstructorInfo GetOptimalConstructor(Type concrete)
		{
			var ctors = concrete
				.GetTypeInfo()
				.GetConstructors()?
				.OrderBy(x => x.GetParameters().Length);

			var optimalCtor = ctors?.Last();

			foreach (var ctor in ctors)
			{
				if (ctor.GetCustomAttribute<PreferredConstructorAttribute>() != null)
				{
					optimalCtor = ctor;
					break;
				}
			}

			if (optimalCtor == null)
			{
				throw new InvalidConstructorException("No valid constructor found for this type");
			}

			return optimalCtor;
		}

		public ITypeBuilder GetTypeBuilder(Profile profile,
			ServiceLifetime lifeCycle)
		{
			ITypeBuilder output = null;

			switch (lifeCycle)
			{
				case ServiceLifetime.Singleton:
					output = new Singleton(profile);
					break;
				case ServiceLifetime.Scoped:
					output = new Scoped(profile);
					break;
				case ServiceLifetime.Transient:
				default:
					output = new Transient(profile);
					break;
			}

			return output;
		}
	}
}

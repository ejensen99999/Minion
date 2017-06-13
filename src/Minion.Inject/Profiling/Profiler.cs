using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Minion.Inject.Builders;
using Minion.Inject.Exceptions;
using Minion.Inject.Interfaces;
using System.Collections.Generic;

namespace Minion.Inject.Profiling
{
	public interface IProfiler
	{
		ConstructorInfo GetOptimalConstructor(Type concrete);

		ITypeBuilder GetTypeBuilder(Profile profile,
			ServiceLifetime lifeCycle);

	    IEnumerable<Type> GetParameters(ConstructorInfo ctor);

	}

	public class Profiler: IProfiler
	{
		public ConstructorInfo GetOptimalConstructor(Type concrete)
		{
            ConstructorInfo output = null;

            if (concrete != null)
		    {
		        var ctors = concrete
		            .GetTypeInfo()
		            .GetConstructors()
		            ?
		            .OrderBy(x => x.GetParameters()
		                .Length);

		        var optimalCtor = ctors?.Last();

		        foreach (var ctor in ctors)
		        {
		            if (ctor.GetCustomAttribute<PreferredConstructorAttribute>() != null)
		            {
		                optimalCtor = ctor;
		                break;
		            }
		        }

		        output = optimalCtor ?? throw new InvalidConstructorException("No valid constructor found for this type");
		    }

		    return output;
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

	    public IEnumerable<Type> GetParameters(ConstructorInfo ctor)
	    {
            var parameters = ctor?
                .GetParameters()?
                .Select(x => x.ParameterType);

	        return parameters;
	    }
	}
}

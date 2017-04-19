using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Minion.Ioc.Exceptions;
using Minion.Ioc.Interfaces;
using Minion.Ioc.Models;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.Extensions.Logging;
using Minion.Ioc.Aspects;

[assembly: InternalsVisibleTo("Minion.Tests")]

namespace Minion.Ioc.Profiler
{
    public class ConstructorProfiler : IConstructorProfile
    {
        private readonly ITypeCache _cache;

        public ConstructorProfiler(ITypeCache cache)
        {
            _cache = cache;
        }

        public ConstructorDefinition GetTargetConstructor(Type contract,
            Type concrete)
        {
            var target = GetTargetType(contract, concrete);
            var ctors = GetConstructors(target);
            var profiles = GetConstructorProfile(ctors);
            var optimal = GetOptimalConstructor(profiles);

            if (optimal == null)
            {
                throw new InvalidConstructorException(
                    $"The concrete type of {contract.FullName} did not have any valid constructors\n" +
                    "Constructor parameters must be a public class and cannot be an enum, optional, or sealed");
            }

            ConvertConstructor(concrete, optimal);

            var constructor = ConstructorEmitter.Emit(target, optimal);
            var output = new ConstructorDefinition(optimal.Parameters, constructor);

            return output;
        }

        internal static Type GetTargetType(Type contract,
            Type concrete)
        {
            var info = contract
                .GetTypeInfo();

            var target = info.IsInterface
                ? concrete
                : contract;

            return target;
        }

        internal static ConstructorInfo[] GetConstructors(Type target)
        {
            var ctors = target
                .GetTypeInfo()
                .GetConstructors();

            return ctors;
        }

        internal static List<IParameterProfile> GetConstructorProfile(ConstructorInfo[] ctors)
        {
            var output = new List<IParameterProfile>();

            foreach (var ctor in ctors)
            {
                if (ctor.GetCustomAttribute<PreferredConstructorAttribute>() != null)
                {
                    output.Clear();
                    output.Add(new ParameterProfiler(ctor));
                    break;
                }
                else
                {
                    output.Add(new ParameterProfiler(ctor));
                }
            }

            return output;
        }

        internal static IParameterProfile GetOptimalConstructor(List<IParameterProfile> profiles)
        {
            IParameterProfile output = null;

            foreach (var profile in profiles)
            {
                if (profile.IsValid && (output == null || profile.Magnitude > output.Magnitude))
                {
                    output = profile;
                }
            }

            return output;
        }

        private void ConvertConstructor(Type concrete, IParameterProfile profile)
        {
            concrete = _cache.GetType(concrete, profile.Ctor);
            profile.Ctor = concrete.GetConstructors().First();
        }
    }
}

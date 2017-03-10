using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Logging;
using Minion.Ioc.Builders;
using Minion.Ioc.Exceptions;
using System.Collections.Generic;
using Minion.Ioc.Interfaces;
using Minion.Ioc.Models;
using Minion.Ioc.Aspects;

namespace Minion.Ioc.Profiler
{
    public class DependencyProfiler : IDependencyProfiler
    {
        private readonly object _synclock;
        private readonly ILogger _log;
        private readonly ConstructorProfiler _ctorProfiler;
        private readonly ConcurrentDictionary<Type, ITypeBuilder> _builders;

        public ConcurrentDictionary<Type, ITypeBuilder> Builders { get { return _builders; } }

        public DependencyProfiler(ILogger log, ConstructorProfiler ctorProfiler)
        {
            _synclock = new object();
            _log = log;
            _ctorProfiler = ctorProfiler;
            _builders = new ConcurrentDictionary<Type, ITypeBuilder>();
        }

        public bool Clean(Guid contextId)
        {
            return _builders.All(x => x.Value.Clean(contextId));
        }

        public void SetMapping<TContract, TConcrete>(Lifetime lifecycle, Func<IServiceProvider, object> initializer)
        {
            SetMapping(typeof(TContract), typeof(TConcrete), lifecycle, initializer);
        }

        public void SetMapping(Type contract, Type concrete, Lifetime lifetime, Func<IServiceProvider, object> initializer)
        {
            try
            {
                if (!_builders.ContainsKey(contract))
                {
                    lock (_synclock)
                    {
                        if (!_builders.ContainsKey(contract))
                        {
                            var ctorDefinition = initializer != null
                                ? new ConstructorDefinition(new List<ParameterDefinition>(), null)
                                : _ctorProfiler.GetTargetConstructor(contract, concrete);

                            var profile = new Profile(_log, contract, concrete, lifetime, initializer, ctorDefinition);

                            _builders.TryAdd(contract, GetTypeBuilder(profile, lifetime));
                        }
                    }
                }
                else
                {
                    throw new IocRegistrationException(
                        $"Type {contract.FullName}: has already been registered with this Ioc container");
                }
            }
            catch (Exception ex)
            {
                _log.LogError(
                    $"Could not establish a proper mapping between {contract.FullName} and {concrete.FullName}", ex);
                throw;
            }
        }

        private ITypeBuilder GetTypeBuilder(Profile profile, Lifetime lifetime)
        {
            ITypeBuilder output = null;

            switch (lifetime)
            {
                case Lifetime.Singleton:
                    output = new Singleton(profile);
                    break;
                case Lifetime.ThreadAsync:
                    output = new ThreadAsync(profile);
                    break;
                case Lifetime.Transient:
                default:
                    output = new Transient(profile);
                    break;
            }

            return output;
        }
    }
}

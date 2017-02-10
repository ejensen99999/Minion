using System;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Linq;
using Minion.Ioc.Profiler;
using Minion.Ioc.Builders;
using Minion.Ioc.Interfaces;

namespace Minion.Ioc
{
    public class Container : IServiceProvider
    {
        private readonly ILogger _log;
        private readonly IDependencyProfiler _profiler;
        private readonly IDependencyResolver _materializer;
        private readonly AsyncLocal<Guid> _contextId;

        public IDependencyProfiler Profiler
        {
            get { return _profiler; }
        }

        public Guid ContextId
        {
            get { return _contextId.Value; }
        }

        public Container(ILogger log, IDependencyProfiler profiler, IDependencyResolver materializer)
        {
            _log = log;
            _profiler = profiler;
            _materializer = materializer;
            _contextId = new AsyncLocal<Guid>();
        }

        public dynamic GetThreadedService(Type serviceType)
        {
            var output = default(object);

            try
            {
                output = _materializer.GetObject(this, serviceType);
            }
            catch (Exception ex)
            {
                _log.LogError($"Could not resolve object: {serviceType.FullName}", ex);
                throw;
            }

            return output;
        }

        public dynamic GetThreadAsyncService<TContract>()
        {
            return GetThreadAsyncService(typeof (TContract));
        }

        public dynamic GetThreadAsyncService(Type serviceType)
        {
            var output = default(object);

            try
            {
                SetContextId();
                output = _materializer.GetObject(this, serviceType);
            }
            catch (Exception ex)
            {
                _log.LogError($"Could not resolve object: {serviceType.FullName}", ex);
                throw;
            }
            finally
            {
                ClearContextId();
            }

            return output;
        }

        public object GetService(Type serviceType)
        {
            var output = default(object);

            try
            {
                output = _materializer.GetObject(this, serviceType);
            }
            catch (Exception ex)
            {
                _log.LogError($"Could not resolve object: {serviceType.FullName}", ex);
                throw;
            }

            return output;
        }

        public TContract Get<TContract>()
            where TContract : class
        {
            var output = GetService(typeof (TContract));

            return (TContract) output;
        }

        public Container ClearContextId()
        {
            _profiler.Clean(_contextId.Value);
            _contextId.Value = new Guid();
            
            return this;
        }

        public Container SetContextId()
        {
            _contextId.Value = Guid.NewGuid();

            return this;
        }

        public Container Add<TConcrete>(Lifetime lifeCycle = Lifetime.Transient)
            where TConcrete : class
        {
            return Add<TConcrete, TConcrete>(lifeCycle);
        }

        public Container Add<TContract, TConcrete>(Lifetime lifecycle = Lifetime.Transient)
            where TConcrete : class, TContract
        {
            _profiler.SetMapping<TContract, TConcrete>(lifecycle, null);

            return this;
        }

        public Container Add<TContract, TConcrete>(TConcrete objectInstance, Lifetime lifecycle = Lifetime.Transient)
            where TConcrete : class, TContract
        {
            _profiler.SetMapping<TContract, TConcrete>(lifecycle, x => objectInstance);

            return this;
        }

        public Container Add<TContract, TConcrete>(Func<IServiceProvider, TConcrete> explicitConstruction, Lifetime lifecycle = Lifetime.Transient)
            where TConcrete : class, TContract
        {
            _profiler.SetMapping<TContract, TConcrete>(lifecycle, explicitConstruction);

            return this;
        }

        public Container AddSingleton<TConcrete>()
            where TConcrete : class
        {
            return AddSingleton<TConcrete, TConcrete>();
        }

        public Container AddSingleton<TContract, TConcrete>()
            where TConcrete : class, TContract
        {
            _profiler.SetMapping<TContract, TConcrete>(Lifetime.Singleton, null);

            return this;
        }

        public Container AddSingleton<TConcrete>(TConcrete objectInstance)
            where TConcrete : class
        {
            return AddSingleton<TConcrete, TConcrete>(objectInstance);
        }

        public Container AddSingleton<TContract, TConcrete>(TConcrete objectInstance)
            where TConcrete : class, TContract
        {
            _profiler.SetMapping<TContract, TConcrete>(Lifetime.Singleton, x => objectInstance);

            return this;
        }

        public Container AddSingleton<TConcrete>(Func<IServiceProvider, TConcrete> explicitConstruction)
            where TConcrete : class
        {
            return AddSingleton<TConcrete, TConcrete>(explicitConstruction);
        }

        public Container AddSingleton<TContract, TConcrete>(Func<IServiceProvider, TConcrete> explicitConstruction)
            where TConcrete : class, TContract
        {
            _profiler.SetMapping<TContract, TConcrete>(Lifetime.Singleton, explicitConstruction);

            return this;
        }

        public Container AddTransient<TConcrete>()
            where TConcrete : class
        {
            return AddTransient<TConcrete, TConcrete>();
        }

        public Container AddTransient<TContract, TConcrete>()
            where TConcrete : class, TContract
        {
            _profiler.SetMapping<TContract, TConcrete>(Lifetime.Transient, null);

            return this;
        }

        public Container AddTransient<TConcrete>(Func<IServiceProvider, TConcrete> explicitConstruction)
            where TConcrete : class
        {
            return AddTransient<TConcrete, TConcrete>(explicitConstruction);
        }

        public Container AddTransient<TContract, TConcrete>(Func<IServiceProvider, TConcrete> explicitConstruction)
            where TConcrete : class, TContract
        {
            _profiler.SetMapping<TContract, TConcrete>(Lifetime.Transient, explicitConstruction);

            return this;
        }

        public Container AddThreadAsync<TConcrete>()
            where TConcrete : class
        {
            return AddThreadAsync<TConcrete, TConcrete>();
        }

        public Container AddThreadAsync<TContract, TConcrete>()
            where TConcrete : class, TContract
        {
            _profiler.SetMapping<TContract, TConcrete>(Lifetime.ThreadAsync, null);

            return this;
        }
    }
}

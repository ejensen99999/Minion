using System;
using System.Collections.Concurrent;

namespace Minion.Ioc.Interfaces
{
    public interface IDependencyProfiler
    {
        ConcurrentDictionary<string, ITypeBuilder> Builders { get; }

        void SetMapping<TContract, TConcrete>(Lifetime lifecycle, Func<IServiceProvider, object> initializer);

        void SetMapping(Type contract, Type concrete, Lifetime lifetime,
            Func<IServiceProvider, object> initializer);

        bool Clean(Guid contextId);
    }
}
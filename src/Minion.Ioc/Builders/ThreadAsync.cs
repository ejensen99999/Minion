using System;
using System.Collections.Concurrent;
using Minion.Ioc.Interfaces;
using Minion.Ioc.Models;

namespace Minion.Ioc.Builders
{
    public class ThreadAsync : Transient, ITypeBuilder
    {
        private readonly ConcurrentDictionary<Guid, dynamic> _threadInstances;

        public ThreadAsync(Profile profile)
            : base(profile)
        {
            _threadInstances = new ConcurrentDictionary<Guid, dynamic>();
        }

        public override dynamic Build(Container container)
        {
            dynamic output = _threadInstances.GetOrAdd(container.ContextId, x =>
            {
                return base.Build(container);
            });

            return output;
        }

        public override bool Clean(Guid contextId)
        {
            dynamic garbage;
            return _threadInstances.TryRemove(contextId, out garbage);
        }
    }
}

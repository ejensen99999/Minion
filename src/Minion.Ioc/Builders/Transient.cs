using System;
using Minion.Ioc.Interfaces;
using Minion.Ioc.Models;

namespace Minion.Ioc.Builders
{
    public class Transient : BaseBuilder, ITypeBuilder
    {
        public Transient(Profile profile)
            : base(profile)
        {

        }

        public virtual dynamic Build(Container container)
        {
            var output = _profile.Initializer != null
                ? _profile.Initializer(container)
                : MaterializeObject(container, _profile);

            return output;
        }

        public virtual bool Clean(Guid contextId)
        {
            return true;
        }
    }
}


using System;
using Minion.Ioc.Interfaces;
using Minion.Ioc.Models;

namespace Minion.Ioc.Builders
{
    public class Singleton: BaseBuilder, ITypeBuilder
    {

        public Singleton(Profile profile)
            : base(profile)
        {

        }

        public dynamic Build(Container container)
        {
            if (_profile.Instance == null)
            {
                lock (_synclock)
                {
                    if (_profile.Instance == null)
                    {
                        _profile.Instance = _profile.Initializer != null
                            ? _profile.Initializer(container)
                            : MaterializeObject(container, _profile);
                    }
                }
            }

            return _profile.Instance;
        }

        public bool Clean(Guid contextId)
        {
            return true;
        }
    }
}

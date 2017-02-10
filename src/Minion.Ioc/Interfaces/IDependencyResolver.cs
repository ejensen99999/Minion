using System;

namespace Minion.Ioc.Interfaces
{
    public interface IDependencyResolver
    {
        object GetObject(Container container,
            Type contract);
    }
}
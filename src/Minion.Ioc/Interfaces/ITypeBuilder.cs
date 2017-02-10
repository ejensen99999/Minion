using System;

namespace Minion.Ioc.Interfaces
{
    public interface ITypeBuilder
    {
        dynamic Build(Container container);
        bool Clean(Guid contextId);
    }
}

using System;

namespace Minion.Injector.Interfaces
{
    public interface ITypeBuilder
    {
        dynamic Build(Container container);
        bool Clean(Guid contextId);
    }
}

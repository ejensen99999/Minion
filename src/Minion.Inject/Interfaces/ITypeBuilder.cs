using System;

namespace Minion.Inject.Interfaces
{
    public interface ITypeBuilder
    {
        dynamic Build(Container container);
        bool Clean(Guid contextId);
    }
}

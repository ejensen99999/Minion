using System.Collections.Generic;

namespace Minion.Ioc.Interfaces
{
    public interface IConstructor
    {
        object Construct(List<object> parameters);
    }
}
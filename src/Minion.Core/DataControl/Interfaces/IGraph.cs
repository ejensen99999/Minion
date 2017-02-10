using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minion.DataControl.Interfaces
{
    public interface IGraph<T,V>
    {
        V this[T key] { get; set; }
    }
}

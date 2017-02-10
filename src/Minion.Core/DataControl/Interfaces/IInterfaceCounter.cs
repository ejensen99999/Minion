using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minion.DataControl.Interfaces
{
    public interface IInterfaceCounter<TObject>
        where TObject: class, new()
    {
        int Count { get; }
        bool IsSet { get; }
        void Hit();
        IInterfaceCounter<TObject> Set(TObject data);
    }
}

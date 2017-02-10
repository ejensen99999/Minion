using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Minion.DataControl.Interfaces
{
    public interface IIndice
    {
        dynamic Id { get; set; }
        IIndice Parent { get; set; }

        bool Blank { get; set; }
        void Clean();

        IIndice Find(params dynamic[] keys);
        IIndice Prune();
        IIndice Prune(dynamic key);
        IEnumerable<dynamic> Slice();

        void SetBlank();
        void SetParentAndId(IIndice parent, dynamic id);
    }
}

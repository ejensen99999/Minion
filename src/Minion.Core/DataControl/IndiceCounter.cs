using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minion.DataControl.Interfaces;

namespace Minion.DataControl
{
    public class IndiceCounter<TObject>: IInterfaceCounter<TObject>
        where TObject : class, new()
    {
        public TObject Data { get; private set; }

        public int Count { get; private set; }

        public bool IsSet
        {
            get { return Data != null; }
        }

        public IInterfaceCounter<TObject> Set(TObject data)
        {
            if (!IsSet)
            {
                Data = data;
            }

            return this;
        }

        public void Hit()
        {
            Count++;
        }
    }
}

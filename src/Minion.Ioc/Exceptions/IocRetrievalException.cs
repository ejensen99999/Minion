using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minion.Ioc.Exceptions
{
    public class IocRetrievalException : Exception
    {
        public IocRetrievalException(string message)
            : base(message)
        {
        }

        public IocRetrievalException(string message,
            Exception exception)
            : base(message, exception)
        {
        }
    }
}

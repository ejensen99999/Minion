using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minion.Ioc.Exceptions
{
    public class InvalidConstructorException: Exception
    {
        public InvalidConstructorException(string message)
            : base(message)
        {
        }

        public InvalidConstructorException(string message,
            Exception exception)
            :base(message, exception)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minion.Ioc.Exceptions
{
    public class IocRegistrationException: Exception
    {
        public IocRegistrationException(string message)
            : base(message)
        {
        }

        public IocRegistrationException(string message,
            Exception exception)
            :base(message, exception)
        {
        }
    }
}
